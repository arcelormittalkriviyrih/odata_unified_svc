using System;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using System.IO;

namespace ODataRestierDynamic.Helpers
{
    /// <summary>
    /// Class for work with Excel template and data of label
    /// </summary>
    public class LabelTemplate
    {
        /// <summary>
        /// The name of the configuration parameter for the dot size.
        /// </summary>
        private const string cDataMatrixDotSizeName = "DataMatrixDotSize";

        /// <summary>
        /// The name of the configuration parameter for the replace image search name.
        /// </summary>
        private const string cReplaceImageSearchNameName = "ReplaceImageSearchName";

        private SpreadsheetDocument spreadSheet = null;
        private WorkbookPart workbookPart = null;
        private Sheet worksheetParams = null;
        private WorksheetPart worksheetPartParams = null;

        /// <summary>	Constructor. </summary>
        ///
        /// <param name="templateName">	Name of the template. </param>
        public LabelTemplate(string templateName)
        {
            spreadSheet = SpreadsheetDocument.Open(templateName, true);
            workbookPart = spreadSheet.WorkbookPart;
            worksheetParams = workbookPart.Workbook.Descendants<Sheet>().First(s => (s.Id == "rId2"));
            worksheetPartParams = (WorksheetPart)(workbookPart.GetPartById(worksheetParams.Id));
        }

        /// <summary>	Inserts a shared string item. </summary>
        ///
        /// <param name="text">			  	The text. </param>
        /// <param name="shareStringPart">	The share string part. </param>
        ///
        /// <returns>	An int. </returns>
        private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }

        /// <summary>	Check empty cell. </summary>
        ///
        /// <param name="currentRow">	The current row. </param>
        /// <param name="afterCell"> 	The after cell. </param>
        /// <param name="columnCell">	The column cell. </param>
        ///
        /// <returns>	A Cell. </returns>
        private Cell CheckEmptyCell(Row currentRow, Cell afterCell, string columnCell)
        {
            Cell returnCell = currentRow.Elements<Cell>().Where(c => c.CellReference.Value == columnCell + currentRow.RowIndex).FirstOrDefault();
            if (returnCell == null)
            {
                returnCell = new Cell() { CellReference = columnCell + currentRow.RowIndex };
                currentRow.InsertAfter(returnCell, afterCell);
            }
            return returnCell;
        }

        /// <summary>	Gets cell value. </summary>
        ///
        /// <param name="cell">	The cell. </param>
        ///
        /// <returns>	The cell value. </returns>
        private string GetCellValue(Cell cell)
        {
            string resultValue = string.Empty;

            if (cell != null)
            {
                resultValue = cell.InnerText;

                if (cell.DataType != null)
                {
                    switch (cell.DataType.Value)
                    {
                        case CellValues.SharedString:
                            // For shared strings, look up the value in the
                            // shared strings table.
                            var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

                            if (stringTable != null)
                            {
                                resultValue = stringTable.SharedStringTable.ElementAt(int.Parse(resultValue)).InnerText;
                            }
                            break;

                        case CellValues.Boolean:
                            switch (resultValue)
                            {
                                case "0":
                                    resultValue = "FALSE";
                                    break;
                                default:
                                    resultValue = "TRUE";
                                    break;
                            }
                            break;
                    }
                }
            }

            return resultValue;
        }

        /// <summary>	Calculates the reference cell values. </summary>
        private void RecalcRefCellValues()
        {
            Sheet sheet1 = workbookPart.Workbook.Descendants<Sheet>().First(s => (s.Id == "rId1"));
            if (sheet1 == null)
                return;

            WorksheetPart wsPartFirst = (WorksheetPart)(workbookPart.GetPartById(sheet1.Id));
            foreach (Cell refCell in wsPartFirst.Worksheet.Descendants<Cell>())
            {
                if ((refCell.DataType == null) && (refCell.CellFormula != null))
                {
                    refCell.CellValue.Remove();
                }
            }
            wsPartFirst.Worksheet.Save();
        }

        /// <summary>
        /// Fill data sheet of parameters
        /// </summary>
        public void FillParamValues(List<PrintPropertiesValue> printPropertiyValues)
        {
            SharedStringTablePart shareStringPart;
            if (spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
            {
                shareStringPart = spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
            }
            else
            {
                shareStringPart = spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart>();
            }

            foreach (Row rowParam in worksheetPartParams.Worksheet.GetFirstChild<SheetData>().Elements<Row>())
            {
                Cell refCellA = rowParam.Elements<Cell>().Where(c => c.CellReference.Value == "A" + rowParam.RowIndex).FirstOrDefault();
                if (refCellA == null)
                {
                    break;
                }
                else
                {
                    Cell refCellB = CheckEmptyCell(rowParam, refCellA, "B");
                    PrintPropertiesValue printPropertiesValue = printPropertiyValues.FirstOrDefault(x => (x.TypeProperty == GetCellValue(refCellA) & (x.PropertyCode == GetCellValue(refCellB))));
                    string PropertyValue = printPropertiesValue == null ? string.Empty : printPropertiesValue.Value;

                    if (!string.IsNullOrEmpty(PropertyValue))
                    {
                        Cell refCellC = CheckEmptyCell(rowParam, refCellB, "C");
                        Cell refCellD = CheckEmptyCell(rowParam, refCellC, "D");

                        if (GetCellValue(refCellB) == "FactoryNumber")
                        {
                            if (!string.IsNullOrEmpty(PropertyValue))
                            {
                                ProcessFactoryNumber(PropertyValue);
                            }
                        }


                        int index = InsertSharedStringItem(PropertyValue, shareStringPart);
                        refCellD.CellValue = new CellValue(index.ToString());
                        refCellD.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                    }
                }
            }
            worksheetPartParams.Worksheet.Save();
            //defective converting? RecalcRefCellValues();
            workbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
            workbookPart.Workbook.CalculationProperties.FullCalculationOnLoad = true;
            workbookPart.Workbook.Save();
            spreadSheet.Close();
        }

        /// <summary>
        /// Generates Data Matrix Code from process factory number and replace it on Excel Sheet 1 
        /// </summary>
        /// <param name="QRvalue">Factory Number value from Cell</param>
        private void ProcessFactoryNumber(string QRvalue)
        {
            //1 - Find Sheet 1 and Sheet 2
            Sheet sheet1 = workbookPart.Workbook.Descendants<Sheet>().First(s => (s.Id == "rId1"));
            if (sheet1 == null)
                return;
            Sheet sheet2 = workbookPart.Workbook.Descendants<Sheet>().First(s => (s.Id == "rId2"));
            if (sheet2 == null)
                return;

            //2 - Find FactoryNumber value on Sheet 2
            string factoryNumberValue = QRvalue;//string.Empty;
                                                //WorksheetPart wsPart2 = (WorksheetPart)(workbookPart.GetPartById(sheet2.Id));
                                                //foreach (Row r in wsPart2.Worksheet.GetFirstChild<SheetData>().Elements<Row>())
                                                //{
                                                //	Cell propCodeCell = r.ElementAt(1) as Cell;
                                                //	string propCodeStr = GetCellValue(workbookPart, propCodeCell);
                                                //	if (propCodeStr == "FactoryNumber")
                                                //	{
                                                //		Cell valueCell = r.ElementAt(3) as Cell;
                                                //		factoryNumberValue = GetCellValue(workbookPart, valueCell);
                                                //		break;
                                                //	}
                                                //}

            string tempImageFileName = System.IO.Path.GetTempFileName();
            if (!string.IsNullOrEmpty(factoryNumberValue))
            {
                //3 - Generate DataMatrix Code from FactoryNumber value
                var enc = new DataMatrix.net.DmtxImageEncoder();
                int dotSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings[cDataMatrixDotSizeName]);
                System.Drawing.Bitmap dataMatrixCode = enc.EncodeImage(factoryNumberValue, dotSize, 0/*margin*/); //dot size 4 (4x4 pixels one point)
                dataMatrixCode.Save(tempImageFileName, System.Drawing.Imaging.ImageFormat.Png);

                //4 - Find all Images with name QRCode
                List<string> imagePartIds = new List<string>();
                WorksheetPart wsPart1 = (WorksheetPart)(workbookPart.GetPartById(sheet1.Id));
                string replaceImageSearchName = System.Configuration.ConfigurationManager.AppSettings[cReplaceImageSearchNameName];
                if (wsPart1 != null && wsPart1.DrawingsPart != null && wsPart1.DrawingsPart.WorksheetDrawing != null)
                {
                    foreach (var element in wsPart1.DrawingsPart.WorksheetDrawing.Elements<TwoCellAnchor>())
                    {
                        foreach (var picture in element.Elements<DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture>())
                        {
                            foreach (var picProp in picture.Elements<DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualPictureProperties>())
                            {
                                foreach (var drawProp in picProp.Elements<DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualDrawingProperties>())
                                {
                                    if (drawProp.Name.Value.StartsWith(replaceImageSearchName, true, System.Globalization.CultureInfo.InvariantCulture))
                                    {
                                        foreach (var blipFill in picture.Elements<DocumentFormat.OpenXml.Drawing.Spreadsheet.BlipFill>())
                                        {
                                            foreach (var blip in blipFill.Elements<DocumentFormat.OpenXml.Drawing.Blip>())
                                            {
                                                if (!string.IsNullOrEmpty(blip.Embed.Value) && !imagePartIds.Contains(blip.Embed.Value))
                                                    imagePartIds.Add(blip.Embed.Value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }


                //5 - Replace all QRCode images
                foreach (var imagePartId in imagePartIds)
                {
                    ImagePart imagePart = (ImagePart)wsPart1.DrawingsPart.GetPartById(imagePartId);
                    using (FileStream fileStream = new FileStream(tempImageFileName, FileMode.Open))
                    {
                        imagePart.FeedData(fileStream);
                    }
                }
            }

            //Clear temp image file
            if (File.Exists(tempImageFileName))
                File.Delete(tempImageFileName);
        }
    }
}

/// <summary>
/// Property values class
/// </summary>
public class PrintPropertiesValue
{
    /// <summary>	Gets or sets the type property. </summary>
    ///
    /// <value>	The type property. </value>
    public string TypeProperty { get; set; }

    /// <summary>	Gets or sets the property code. </summary>
    ///
    /// <value>	The property code. </value>
    public string PropertyCode { get; set; }

    /// <summary>	Gets or sets the value. </summary>
    ///
    /// <value>	The value. </value>
    public string Value { get; set; }
}