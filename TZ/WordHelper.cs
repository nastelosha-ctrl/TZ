using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;

namespace TZ
{
    /// <summary>
    /// Вспомогательный класс для создания чеков заказов в формате Microsoft Word
    /// Формирует документ в стиле типографского чека с информацией о заказе, клиенте и услугах
    /// </summary>
    public static class WordHelper
    {
        /// <summary>
        /// Создает чек заказа в формате Word в стиле типографии
        /// </summary>
        public static void CreateReceipt(int orderId, string clientFIO, string clientPhone,
                                        DateTime admissionDate, DateTime dueDate,
                                        DataTable services, bool discountApplied)
        {
            Word.Application wordApp = null;
            Word.Document doc = null;

            try
            {
                // Создаем приложение Word
                wordApp = new Word.Application();
                wordApp.Visible = false;

                // Создаем новый документ
                doc = wordApp.Documents.Add();

                // НАСТРОЙКА СТРАНИЦЫ ДЛЯ МАКСИМАЛЬНОЙ ЭКОНОМИИ МЕСТА
                doc.PageSetup.TopMargin = 10f;      // Минимальные поля
                doc.PageSetup.BottomMargin = 10f;
                doc.PageSetup.LeftMargin = 15f;
                doc.PageSetup.RightMargin = 15f;

                // Получаем объект Range
                Word.Range range = doc.Content;

                // СБРАСЫВАЕМ ВСЕ ФОРМАТИРОВАНИЯ АБЗАЦА
                range.ParagraphFormat.SpaceBefore = 0f;
                range.ParagraphFormat.SpaceAfter = 0f;
                range.ParagraphFormat.LineSpacingRule = Word.WdLineSpacing.wdLineSpaceSingle;
                range.ParagraphFormat.LineSpacing = 12f; // Фиксированный интервал
                range.ParagraphFormat.WidowControl = 0;  // Отключаем контроль висячих строк
                range.ParagraphFormat.KeepTogether = 0;  // Разрешаем разрывы страниц
                range.ParagraphFormat.KeepWithNext = 0;  // Не привязываем к следующему
                range.ParagraphFormat.PageBreakBefore = 0; // Без принудительных разрывов

                // НАСТРОЙКА ШРИФТА
                range.Font.Name = "Courier New";
                range.Font.Size = 7; // Очень мелкий шрифт
                range.Font.Bold = 0; // Убираем жирность
                range.Font.Italic = 0; // Убираем курсив
                range.Font.Underline = Word.WdUnderline.wdUnderlineNone; // Убираем подчеркивание
                range.Font.Spacing = 0f; // Нормальный межбуквенный интервал
                range.Font.Scaling = 100; // Нормальный масштаб
                range.Font.Kerning = 0f; // Отключаем кернинг

                // Заполняем документ
                AddLine(range, "ТИПОГРАФИЯ \"ПЕЧАТНЫЙ МАСТЕРСКАЯ\"");
                AddLine(range, "г. Москва, ул. Полиграфическая, д. 10");
                AddLine(range, "Тел: +7 (495) 123-45-67");
                AddLine(range, "========================================");
                AddEmptyLine(range, false);

                AddLine(range, $"ЗАКАЗ №: {orderId}");
                AddLine(range, $"ДАТА: {admissionDate:dd.MM.yyyy}");
                AddEmptyLine(range, true);

                AddLine(range, "КЛИЕНТ:");
                AddLine(range, $"{clientFIO}");
                AddLine(range, $"Тел: {clientPhone}");
                AddEmptyLine(range, true);

                string managerName = CurrentUser.FIO;
                if (string.IsNullOrEmpty(managerName))
                    managerName = "Не указан";
                AddLine(range, "МЕНЕДЖЕР:");
                AddLine(range, $"{managerName}");
                AddEmptyLine(range, true);

                AddLine(range, $"СРОК ВЫПОЛНЕНИЯ: {dueDate:dd.MM.yyyy}");
                AddLine(range, "========================================");
                AddEmptyLine(range, false);

                AddLine(range, "УСЛУГИ:");
                AddEmptyLine(range, false);

                decimal totalSum = 0;
                for (int i = 0; i < services.Rows.Count; i++)
                {
                    DataRow row = services.Rows[i];
                    string serviceName = row["service_name"].ToString();
                    decimal price = Convert.ToDecimal(row["price"]);
                    totalSum += price;

                    if (serviceName.Length > 30)
                        serviceName = serviceName.Substring(0, 27) + "...";

                    // Формируем строку с пробелами
                    string line = serviceName;
                    int spacesCount = 35 - serviceName.Length;
                    if (spacesCount > 0)
                        line += new string(' ', spacesCount);
                    line += $" {price:N2} руб.";

                    AddLine(range, line);
                }

                AddEmptyLine(range, false);
                AddLine(range, "----------------------------------------");

                AddLine(range, $"ИТОГО: {totalSum:N2} руб.");

                if (discountApplied)
                {
                    decimal discountedSum = totalSum * 0.9m;
                    decimal discountAmount = totalSum - discountedSum;

                    AddEmptyLine(range, false);
                    AddLine(range, $"СКИДКА 10%: -{discountAmount:N2} руб.");
                    AddLine(range, "----------------------------------------");
                    AddLine(range, $"К ОПЛАТЕ: {discountedSum:N2} руб.");
                }
                else
                {
                    AddLine(range, "----------------------------------------");
                    AddLine(range, $"К ОПЛАТЕ: {totalSum:N2} руб.");
                }

                AddEmptyLine(range, false);
                AddEmptyLine(range, false);

                AddLine(range, "СТАТУС: ПРИНЯТ");
                AddLine(range, "--------------");
                AddEmptyLine(range, false);

                AddLine(range, "ПОДПИСЬ МЕНЕДЖЕРА:");
                AddEmptyLine(range, false);
                AddLine(range, "_________________________");
                AddEmptyLine(range, true);

                AddLine(range, "СПАСИБО ЗА ЗАКАЗ!");
                AddLine(range, "Ждем вас снова!");

                // Сохраняем документ
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fileName = $"Check_{orderId}_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
                string filePath = Path.Combine(desktopPath, fileName);

                doc.SaveAs(filePath);
                doc.Close();
                wordApp.Quit();

                if (File.Exists(filePath))
                {
                    MessageBox.Show($"Чек сохранен:\n{filePath}", "Успех");
                    System.Diagnostics.Process.Start("explorer.exe", $"/select, \"{filePath}\"");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
                if (doc != null) try { doc.Close(false); } catch { }
                if (wordApp != null) try { wordApp.Quit(); } catch { }
            }
            finally
            {
                if (doc != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
                if (wordApp != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
            }
        }

        /// <summary>
        /// Добавляет строку с минимальным форматированием
        /// </summary>
        private static void AddLine(Word.Range range, string text)
        {
            Word.Paragraph para = range.Paragraphs.Add();
            para.Range.Text = text;
            para.Range.Font.Size = 7;
            para.Range.Font.Name = "Courier New";
            para.Range.Font.Bold = 0;
            para.Range.ParagraphFormat.SpaceBefore = 0f;
            para.Range.ParagraphFormat.SpaceAfter = 0f;
            para.Range.ParagraphFormat.LineSpacing = 10f; // Минимальный интервал
            para.Range.ParagraphFormat.LineSpacingRule = Word.WdLineSpacing.wdLineSpaceExactly; // Точно заданный
            para.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            para.Range.InsertParagraphAfter();
        }

        /// <summary>
        /// Добавляет пустую строку с контролем интервала
        /// </summary>
        private static void AddEmptyLine(Word.Range range, bool addSpace)
        {
            if (!addSpace) return;

            Word.Paragraph para = range.Paragraphs.Add();
            para.Range.Text = "";
            para.Range.Font.Size = 7;
            para.Range.ParagraphFormat.SpaceBefore = 0f;
            para.Range.ParagraphFormat.SpaceAfter = 0f;
            para.Range.ParagraphFormat.LineSpacing = 10f;
            para.Range.ParagraphFormat.LineSpacingRule = Word.WdLineSpacing.wdLineSpaceExactly;
            para.Range.InsertParagraphAfter();
        }
    }
}