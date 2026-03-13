using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TZ
{
    /// <summary>
    /// Класс для генерации CAPTCHA изображений
    /// </summary>
    public class CaptchaGenerator
    {
        private static Random random = new Random();

        /// <summary>
        /// Генерирует случайный текст CAPTCHA (4 символа)
        /// </summary>
        public static string GenerateText()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            char[] result = new char[4];

            for (int i = 0; i < 4; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }

            return new string(result);
        }

        /// <summary>
        /// Создает изображение CAPTCHA с наложенными символами и шумом
        /// </summary>
        public static Image CreateImage(string captchaText, int width = 250, int height = 80)
        {
            Bitmap bitmap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bitmap);

            // Настройки графики
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            // Заливаем фон случайным светлым цветом
            Color backColor = Color.FromArgb(240 + random.Next(15), 240 + random.Next(15), 240 + random.Next(15));
            g.Clear(backColor);

            // Добавляем шум (случайные линии)
            for (int i = 0; i < 20; i++)
            {
                Pen pen = new Pen(Color.FromArgb(random.Next(100, 200), random.Next(100, 200), random.Next(100, 200)), random.Next(1, 3));
                int x1 = random.Next(width);
                int y1 = random.Next(height);
                int x2 = random.Next(width);
                int y2 = random.Next(height);
                g.DrawLine(pen, x1, y1, x2, y2);
            }

            // Добавляем шум (точки)
            for (int i = 0; i < 200; i++)
            {
                int x = random.Next(width);
                int y = random.Next(height);
                bitmap.SetPixel(x, y, Color.FromArgb(random.Next(100, 200), random.Next(100, 200), random.Next(100, 200)));
            }

            // Рисуем символы с наложением и перечеркиванием
            Random rand = new Random();

            for (int i = 0; i < captchaText.Length; i++)
            {
                char c = captchaText[i];

                // Случайный шрифт
                string[] fonts = { "Arial", "Verdana", "Times New Roman", "Courier New", "Comic Sans MS" };
                Font font = new Font(fonts[rand.Next(fonts.Length)], rand.Next(25, 35), FontStyle.Bold);

                // Случайный цвет
                Color color = Color.FromArgb(
                    rand.Next(50, 200),
                    rand.Next(50, 200),
                    rand.Next(50, 200)
                );
                Brush brush = new SolidBrush(color);

                // Случайная позиция с наложением
                int x = 30 + i * 40 + rand.Next(-10, 10);
                int y = 20 + rand.Next(-10, 10);

                // Поворот символа
                g.RotateTransform(rand.Next(-25, 25));

                // Рисуем символ
                g.DrawString(c.ToString(), font, brush, x, y);

                // Возвращаем трансформацию
                g.ResetTransform();

                // Рисуем перечеркивающую линию
                Pen linePen = new Pen(Color.FromArgb(rand.Next(150, 255), rand.Next(0, 100), rand.Next(0, 100)), 2);
                int lineX1 = x + rand.Next(-5, 5);
                int lineY1 = y + rand.Next(-5, 5);
                int lineX2 = x + 30 + rand.Next(-5, 5);
                int lineY2 = y + 30 + rand.Next(-5, 5);
                g.DrawLine(linePen, lineX1, lineY1, lineX2, lineY2);
            }

            // Добавляем рамку
            g.DrawRectangle(new Pen(Color.Gray, 1), 0, 0, width - 1, height - 1);

            return bitmap;
        }

        /// <summary>
        /// Генерирует новую CAPTCHA (текст + изображение)
        /// </summary>
        public static CaptchaResult Generate()
        {
            string text = GenerateText();
            Image image = CreateImage(text);
            return new CaptchaResult { Text = text, Image = image };
        }
    }

    /// <summary>
    /// Результат генерации CAPTCHA
    /// </summary>
    public class CaptchaResult
    {
        public string Text { get; set; }
        public Image Image { get; set; }
    }
}