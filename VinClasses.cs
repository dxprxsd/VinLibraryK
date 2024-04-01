using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinLibrary
{
    // класс VIN
    public class VIN
    {
        // поля класса VIN
        public string FullVIN { get; private set; } 
        public string WMI { get; private set; }
        public string VDS { get; private set; }
        public string VIS { get; private set; }
        // private set на поля установлен, так как по задумке их нельзя изменять снаружи этого класса

        // конструктор класса VIN
        public VIN(string vin)
        {
            VINValidator.ValidateVIN(vin);

            FullVIN = vin;
            WMI = vin.Substring(0, 3);
            VDS = vin.Substring(3, 6);
            VIS = vin.Substring(9);
        }
    }

    // статический класс для валидации VIN
    public static class VINValidator
    {
        // поля класса VINValidator
        private static readonly string LegalCharacters = "0 1 2 3 4 5 6 7 8 9 A B C D E F G H J K L M N P R S T U V W X Y Z";
        private static readonly string CheckSumLegalCharacters = "0 1 2 3 4 5 6 7 8 9 X";
        // private доступ к полям, так как они не нужны вне этого класса
        // readonly, потому что это константы


        // публичный метод для валидации VIN
        public static void ValidateVIN(string vin)
        {
            // проверка на корректную длину
            if (!HasCorrectLength(vin))
            {
                throw new ArgumentException("Incorrect length of VIN.");
            }

            // проверка на разрешенные символы
            if (!ContainsOnlyLegalCharacters(vin))
            {
                throw new ArgumentException("VIN contains illegal characters.");
            }

            // проверка на разрешенный символ контрольной суммы
            if (!HasLegalCheckSumCharacter(vin))
            {
                throw new ArgumentException("VIN checksum character is illegal.");
            }

            // проверка значения контрольной суммы
            if (!IsValidCheckSum(vin))
            {
                throw new ArgumentException("VIN has invalid checksum.");
            }
        } 


        // внутренний (private) метод класса для проверки значения контрольной суммы
        private static bool IsValidCheckSum(string vin)
        {
            // В VIN на девятой позиции (восьмом индексе) расположен символ контрольной суммы
            char CheckSumSymbol = vin[9 - 1];

            // словарь: символ (char) -> цифровой эквивалент (int)
            // ключ - символ (char), а значение - цифровой эквивалент (int)
            Dictionary<char, int> LetterToNumber = new Dictionary<char, int>
            {
                { 'A', 1 }, { 'B', 2 }, { 'C', 3 }, { 'D', 4 }, { 'E', 5 }, { 'F', 6 }, { 'G', 7 }, { 'H', 8 },
                { 'J', 1 }, { 'K', 2 }, { 'L', 3 }, { 'M', 4 }, { 'N', 5 }, { 'P', 7 }, { 'R', 9 },
                { 'S', 2 }, { 'T', 3 }, { 'U', 4 }, { 'V', 5 }, { 'W', 6 }, { 'X', 7 }, { 'Y', 8 }, { 'Z', 9 }
            };

            // словарь: позиция (int) -> вес (int)
            // ключ - позиция (int), а значение - вес (int)
            Dictionary<int, int> PositionToWeight = new Dictionary<int, int>
            {
                { 1, 8 }, { 2, 7 }, { 3, 6 } , { 4, 5 }, { 5, 4 }, { 6, 3 }, { 7, 2 }, { 8, 10 }, { 9, 0 }, 
                { 10, 9 }, { 11, 8 }, { 12, 7 }, { 13, 6 }, { 14, 5 }, { 15, 4 }, { 16, 3 }, { 17, 2 }
            };
            
            int CalculatedCheckSum = 0;

            for (int i = 0; i < vin.Length; i++)
            {
                int Number;
                if (char.IsDigit(vin[i]))
                {
                    // если символ - это цифра, то его цифровой эквивалент будет равен этому же числу
                    Number = vin[i] - '0';
                }
                else
                {
                    // если символ - это не цифра, то используем словарь LetterToNumber для получения цифрового эквивалента
                    Number = LetterToNumber[vin[i]];
                }

                CalculatedCheckSum += Number * PositionToWeight[i + 1]; // i + 1, потому что i - это индекс, а нам нужна позиция (+1)
            }

            int ClosestValue = CalculatedCheckSum / 11; // делим на 11 нацело
            ClosestValue *= 11; 

            int CalculatedCheckSumValue = CalculatedCheckSum - ClosestValue; 

            char CalculatedCheckSumSymbol;

            if (CalculatedCheckSumValue != 10)
            {
                // если значение контрольной суммы не равно 10, значит оно от 0 до 9
                // а значит символ будет равен (char) ('0' + CalculatedCheckSumValue)
                // (char) ('0' + 0) = '0'
                // (char) ('0' + 3) = '3'
                CalculatedCheckSumSymbol = (char) ('0' + CalculatedCheckSumValue);
            }
            else
            {
                // если контрольная сумма равна 10, то символом будет 'X'
                CalculatedCheckSumSymbol = 'X';
            }


            // проверяем, равен ли символ контрольной суммы с вычисленным символом контрольной суммы
            if (CheckSumSymbol == CalculatedCheckSumSymbol)
            {
                return true;
            }

            return false;
        }

        // внутренний (private) метод класса для проверки на разрешенные символы VIN
        private static bool ContainsOnlyLegalCharacters(string vin)
        {
            for (int i = 0; i < vin.Length; i++)
            {
                // условие для корректного символа:
                // (vin[i] != ' ' && LegalCharacters.Contains(vin[i]))

                // текущий символ должен быть:
                // 1) не равен пробелу (потому что в строке LegalCharacters есть пробелы для разделения символов)
                // 2) строка LegalCharacters должна содержать в себе (Contains) текущий символ

                // на это условие наложена инверсия, то есть мы получаем условие для некорректного символа

                if (!(vin[i] != ' ' && LegalCharacters.Contains(vin[i]))) {
                    return false; // false при некорректном символе
                }
            }

            return true;
        }

        // внутренний (private) метод класса для проверки на разрешенный символ контрольной суммы (не значения)
        private static bool HasLegalCheckSumCharacter(string vin)
        {
            // В VIN на девятой позиции (восьмом индексе) расположен символ контрольной суммы
            char CheckSumCharacter = vin[9 - 1];

            // Строка CheckSumLegalCharacters содержит все разрешенные символы контрольной суммы, разделенные пробелами
            if (!CheckSumLegalCharacters.Contains(CheckSumCharacter))
            {
                return false;
            }

            return true;
        }

        // внутренний (private) метод класса для проверки длины VIN
        private static bool HasCorrectLength(string vin)
        {
            if (vin.Length == 17)
            {
                return true;
            }
            return false;
        }
    }

    // статический класс для описания VIN
    // здесь описан шаблон того, что можно сделать

    // методы можно сделать рабочими, если перенести реальные таблицы из .pdf к заданию и из интернета,
    // но это не предусмотрено рамками задания, поэтому оставим это шаблоном
    public static class VINDescription
    {

        // WMI состоит из трёх знаков и однозначно определяет изготовителя ТС.
        // Первый знак указывает географическую зону, 
        // второй знак(совместно с первым) — страну в этой зоне,
        // третий — конкретного изготовителя автомобиля
        public static string GetManufacturerCountry()
        {
            return "ManufacturerCountry";
        }

        public static string GetManufacturer()
        {
            return "Manufacturer";
        }


        // VDS состоит из шести знаков и описывает характеристики автомобиля. 
        // Последовательность знаков и заложенные в них характеристики определяются изготовителем.
        // Обычно здесь заложены сведения о модели автомобиля, типе кузова, комплектации, двигателе и т.д.
        public static string GetVehicleParameters()
        {
            return "Parameters";
        }   

        // Обычно первый знак VIS (10-й знак VIN) несёт в себе сведения о модельном годе автомобиля
        public static string GetModelYear()
        {
            return "Year";
        }

        // Второй знак VIS (11-й знак VIN) чаще всего содержит сведения о заводе-изготовителе данного ТС.
        public static string GetFactoryCode()
        {
            return "FactoryCode";
        }

        // Последние 6 символов VIS содержат серийный номер
        public static string GetSerialNumber(VIN vin)
        {
            string SerialNumber = vin.VIS.Substring(2);
            return SerialNumber;
        }
    }

}
