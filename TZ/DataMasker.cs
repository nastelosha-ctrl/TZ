using System;
using System.Linq;

namespace TZ
{
    /// <summary>
    /// Класс для маскирования персональных данных
    /// </summary>
    public static class DataMasker
    {
        /// <summary>
        /// Маскирование ФИО (оставляет первые 3 буквы фамилии, имени и отчества)
        /// Пример: "Иванов Иван Петрович" → "Ива*** Ив*** Пет****"
        /// </summary>
        public static string MaskFIO(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return "";

            string[] parts = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return "";

            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = MaskWord(parts[i], 3); // Оставляем первые 3 буквы
            }

            return string.Join(" ", parts);
        }

        /// <summary>
        /// Маскирование отдельного слова
        /// </summary>
        /// <param name="word">Слово для маскирования</param>
        /// <param name="visibleCount">Количество видимых символов</param>
        private static string MaskWord(string word, int visibleCount = 3)
        {
            if (string.IsNullOrEmpty(word))
                return "";

            if (word.Length <= visibleCount)
                return word;

            string visible = word.Substring(0, visibleCount);
            string masked = new string('*', word.Length - visibleCount);

            return visible + masked;
        }

        /// <summary>
        /// Маскирование номера телефона
        /// Формат: +7 (904) 356-57-67 → +7 (904) ***-**-67
        /// Оставляет код оператора и последние 2-3 цифры
        /// </summary>
        public static string MaskPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return "";

            // Оставляем только цифры
            string digits = new string(phone.Where(char.IsDigit).ToArray());

            if (digits.Length < 10)
                return phone;

            // Для российских номеров (11 цифр с +7)
            if (digits.Length == 11)
            {
                string code = digits.Substring(1, 3);      // код оператора (904)
                string lastDigits = digits.Substring(8, 3); // последние 3 цифры (67 из 57-67)

                return $"+7 ({code}) ***-**-{lastDigits}";
            }

            // Если формат другой, оставляем первые 4 и последние 3 цифры
            if (digits.Length > 7)
            {
                string firstPart = digits.Substring(0, 4);
                string lastPart = digits.Substring(digits.Length - 3);
                return $"{firstPart}***{lastPart}";
            }

            // Если номер короткий, маскируем середину
            int visibleStart = Math.Min(3, digits.Length / 2);
            int visibleEnd = Math.Min(2, digits.Length / 3);
            string start = digits.Substring(0, visibleStart);
            string end = digits.Substring(digits.Length - visibleEnd);
            return start + new string('*', digits.Length - visibleStart - visibleEnd) + end;
        }
    }
}