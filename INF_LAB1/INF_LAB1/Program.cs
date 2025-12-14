using System;

namespace SLAE_Solver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Решение СЛАУ - Вариант 16";
            Console.WriteLine("=== РЕШЕНИЕ СЛАУ (Вариант 16) ===");
            Console.WriteLine("Система уравнений:");
            Console.WriteLine("1) 7.7x₁ + 5.2x₂ + 4.9x₃ = 1.8");
            Console.WriteLine("2) 3.1x₁ + 1.2x₂ + 1.8x₃ = 2.3");
            Console.WriteLine("3) 4.5x₁ + 3.2x₂ + 2.8x₃ = 3.4");
            Console.WriteLine();

            double[,] A = {
                { 7.7, 5.2, 4.9 },
                { 3.1, 1.2, 1.8 },
                { 4.5, 3.2, 2.8 }
            };

            double[] B = { 1.8, 2.3, 3.4 };

            // Главное меню
            bool continueRunning = true;

            while (continueRunning)
            {
                Console.Clear();
                PrintHeader();

                Console.WriteLine("\nМЕНЮ");
                Console.WriteLine("1. Матричный метод");
                Console.WriteLine("2. Метод Гаусса");
                Console.WriteLine("3. Метод Якоби");
                Console.WriteLine("4. Метод Зейделя");
                Console.WriteLine("5. Все методы");
                Console.WriteLine("6. Сравнить результаты");
                Console.WriteLine("7. Проверить матрицу");
                Console.WriteLine("8. Выход");
                Console.Write("\nВыберите метод (1-8): ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RunMatrixMethod(A, B);
                        break;
                    case "2":
                        RunGaussMethod(A, B);
                        break;
                    case "3":
                        RunJacobiMethod(A, B);
                        break;
                    case "4":
                        RunSeidelMethod(A, B);
                        break;
                    case "5":
                        RunAllMethods(A, B);
                        break;
                    case "6":
                        CompareAllMethods(A, B);
                        break;
                    case "7":
                        CheckMatrixProperties(A);
                        break;
                    case "8":
                        continueRunning = false;
                        Console.WriteLine("\nПрограмма завершена.");
                        break;
                    default:
                        Console.WriteLine("\nНекорректный выбор! Нажмите любую клавишу...");
                        Console.ReadKey();
                        break;
                }

                if (continueRunning && choice != "8")
                {
                    Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
                    Console.ReadKey();
                }
            }
        }

        static void PrintHeader()
        {
            Console.WriteLine("       РЕШЕНИЕ СИСТЕМЫ ЛИНЕЙНЫХ        ");
            Console.WriteLine("      АЛГЕБРАИЧЕСКИХ УРАВНЕНИЙ      ");
        }

        static double GetEpsilonFromUser(string methodName)
        {
            Console.WriteLine($"\n=== {methodName} ===");

            while (true)
            {
                try
                {
                    Console.Write("\nВведите точность (например: 0.001, 1e-6) или 'q' для отмены: ");
                    string input = Console.ReadLine().Trim().ToLower();

                    if (input == "q" || input == "выход" || input == "отмена")
                    {
                        return -1; // Специальное значение для отмены
                    }

                    if (double.TryParse(input.Replace(',', '.'), System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out double eps))
                    {
                        if (eps <= 0)
                        {
                            Console.WriteLine("Ошибка: Точность должна быть положительным числом!");
                            continue;
                        }

                        if (eps > 0.1)
                        {
                            Console.WriteLine("Внимание: Выбрана низкая точность. Рекомендуется < 0.001");
                            Console.Write("Продолжить? (да/нет): ");
                            if (Console.ReadLine().Trim().ToLower() != "да")
                                continue;
                        }

                        return eps;
                    }
                    else
                    {
                        Console.WriteLine("Ошибка: Введите корректное число!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка ввода: {ex.Message}");
                }
            }
        }

        static int GetMaxIterationsFromUser()
        {
            while (true)
            {
                try
                {
                    Console.Write("Введите максимальное число итераций (100-10000): ");
                    string input = Console.ReadLine().Trim();

                    if (int.TryParse(input, out int maxIter))
                    {
                        if (maxIter < 10)
                        {
                            Console.WriteLine("Слишком мало итераций! Минимум 10.");
                            continue;
                        }

                        if (maxIter > 100000)
                        {
                            Console.WriteLine("Очень много итераций! Максимум 100000.");
                            continue;
                        }

                        return maxIter;
                    }
                    else
                    {
                        Console.WriteLine("Ошибка: Введите целое число!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка ввода: {ex.Message}");
                }
            }
        }

        static void RunMatrixMethod(double[,] A, double[] B)
        {
            Console.Clear();
            PrintHeader();
            Console.WriteLine("\n=== МАТРИЧНЫЙ МЕТОД ===");

            try
            {
                double[] solution = MatrixMethod(A, B);
                PrintSolutionTable(solution, "Матричный метод");
                CheckSolution(A, B, solution, "Матричный метод");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка при вычислении: {ex.Message}");
            }
        }

        static void RunGaussMethod(double[,] A, double[] B)
        {
            Console.Clear();
            PrintHeader();
            Console.WriteLine("\n=== МЕТОД ГАУССА ===");

            try
            {
                double[] solution = GaussMethod(A, B);
                PrintSolutionTable(solution, "Метод Гаусса");
                CheckSolution(A, B, solution, "Метод Гаусса");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка при вычислении: {ex.Message}");
            }
        }

        static void RunJacobiMethod(double[,] A, double[] B)
        {
            Console.Clear();
            PrintHeader();

            double eps = GetEpsilonFromUser("МЕТОД ЯКОБИ");
            if (eps < 0) return; // Пользователь отменил

            int maxIter = GetMaxIterationsFromUser();

            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("НАЧАЛО ВЫЧИСЛЕНИЙ МЕТОДОМ ЯКОБИ");
            Console.WriteLine(new string('=', 50));

            // Проверка сходимости
            CheckDiagonalDominance(A);

            try
            {
                Console.WriteLine("\nТаблица итераций:");
                Console.WriteLine(new string('-', 70));
                Console.WriteLine("| Итер. |      x1       |      x2       |      x3       |  Погрешность  |");
                Console.WriteLine(new string('-', 70));

                double[] solution = JacobiMethodWithDetails(A, B, eps, maxIter, true);

                Console.WriteLine(new string('-', 70));
                PrintSolutionTable(solution, "Метод Якоби");
                CheckSolution(A, B, solution, "Метод Якоби");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка при вычислении: {ex.Message}");
            }
        }

        static void RunSeidelMethod(double[,] A, double[] B)
        {
            Console.Clear();
            PrintHeader();

            double eps = GetEpsilonFromUser("МЕТОД ЗЕЙДЕЛЯ");
            if (eps < 0) return; // Пользователь отменил

            int maxIter = GetMaxIterationsFromUser();

            Console.WriteLine("НАЧАЛО ВЫЧИСЛЕНИЙ МЕТОДОМ ЗЕЙДЕЛЯ");
            Console.WriteLine(new string('=', 50));

            // Проверка сходимости
            CheckDiagonalDominance(A);

            try
            {
                Console.WriteLine("\nТаблица итераций:");
                Console.WriteLine(new string('-', 70));
                Console.WriteLine("| Итер. |      x1       |      x2       |      x3       |  Погрешность  |");
                Console.WriteLine(new string('-', 70));

                double[] solution = SeidelMethodWithDetails(A, B, eps, maxIter, true);

                Console.WriteLine(new string('-', 70));
                PrintSolutionTable(solution, "Метод Зейделя");
                CheckSolution(A, B, solution, "Метод Зейделя");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка при вычислении: {ex.Message}");
            }
        }

        static void RunAllMethods(double[,] A, double[] B)
        {
            Console.Clear();
            PrintHeader();
            Console.WriteLine("\n=== ВСЕ МЕТОДЫ ===");

            Console.WriteLine("\n1. МАТРИЧНЫЙ МЕТОД:");
            Console.WriteLine(new string('-', 40));
            try
            {
                double[] xMatrix = MatrixMethod(A, B);
                PrintSolutionTable(xMatrix, "Матричный метод");
            }
            catch (Exception ex) { Console.WriteLine($"Ошибка: {ex.Message}"); }

            Console.WriteLine("\n2. МЕТОД ГАУССА:");
            Console.WriteLine(new string('-', 40));
            try
            {
                double[] xGauss = GaussMethod(A, B);
                PrintSolutionTable(xGauss, "Метод Гаусса");
            }
            catch (Exception ex) { Console.WriteLine($"Ошибка: {ex.Message}"); }

            Console.WriteLine("\n3. МЕТОД ЯКОБИ (быстрый запуск):");
            Console.WriteLine(new string('-', 40));
            Console.WriteLine("Используются параметры по умолчанию: eps=0.0001, maxIter=100");
            try
            {
                double[] xJacobi = JacobiMethod(A, B, 0.0001, 100);
                PrintSolutionTable(xJacobi, "Метод Якоби");
            }
            catch (Exception ex) { Console.WriteLine($"Ошибка: {ex.Message}"); }

            Console.WriteLine("\n4. МЕТОД ЗЕЙДЕЛЯ (быстрый запуск):");
            Console.WriteLine(new string('-', 40));
            Console.WriteLine("Используются параметры по умолчанию: eps=0.0001, maxIter=100");
            try
            {
                double[] xSeidel = SeidelMethod(A, B, 0.0001, 100);
                PrintSolutionTable(xSeidel, "Метод Зейделя");
            }
            catch (Exception ex) { Console.WriteLine($"Ошибка: {ex.Message}"); }
        }

        static void CompareAllMethods(double[,] A, double[] B)
        {
            Console.Clear();
            PrintHeader();
            Console.WriteLine("\n=== СРАВНЕНИЕ РЕЗУЛЬТАТОВ ===");

            try
            {
                double[] xMatrix = MatrixMethod(A, B);
                double[] xGauss = GaussMethod(A, B);
                double[] xJacobi = JacobiMethod(A, B, 0.0001, 100);
                double[] xSeidel = SeidelMethod(A, B, 0.0001, 100);

                Console.WriteLine("\n" + new string('-', 120));
                Console.WriteLine("| Метод          |       x1       |       x2       |       x3       | Проверка 1 ур. | Проверка 2 ур. | Проверка 3 ур. |");
                Console.WriteLine(new string('-', 120));

                PrintComparisonRow("Матричный", xMatrix, A, B);
                PrintComparisonRow("Гаусса", xGauss, A, B);
                PrintComparisonRow("Якоби", xJacobi, A, B);
                PrintComparisonRow("Зейделя", xSeidel, A, B);

                Console.WriteLine(new string('-', 120));

                // Сравнение точности
                Console.WriteLine("\nСравнение с матричным методом (эталон):");
                CompareWithReference(xMatrix, xGauss, "Гаусса");
                CompareWithReference(xMatrix, xJacobi, "Якоби");
                CompareWithReference(xMatrix, xSeidel, "Зейделя");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сравнении: {ex.Message}");
            }
        }

        static void CheckMatrixProperties(double[,] A)
        {
            Console.Clear();
            PrintHeader();
            Console.WriteLine("\n=== АНАЛИЗ МАТРИЦЫ ===");

            double det = Determinant(A);
            Console.WriteLine($"\n1. Определитель матрицы: {det:E6} ({det:F6})");

            if (Math.Abs(det) < 1e-10)
                Console.WriteLine("   ⚠️  Матрица близка к вырожденной!");
            else
                Console.WriteLine("   ✓ Матрица невырожденная");

            Console.WriteLine("\n2. Проверка диагонального преобладания:");
            CheckDiagonalDominance(A);

            Console.WriteLine("\n3. Число обусловленности (оценка):");
            EstimateConditionNumber(A);

            Console.WriteLine("\n4. Симметричность:");
            CheckSymmetry(A);
        }

        // МЕТОДЫ РЕШЕНИЯ (аналогичны предыдущим, но с улучшениями)

        static double[] MatrixMethod(double[,] A, double[] B)
        {
            int n = B.Length;
            double[,] invA = InverseMatrix(A);
            double[] X = new double[n];

            for (int i = 0; i < n; i++)
            {
                X[i] = 0;
                for (int j = 0; j < n; j++)
                {
                    X[i] += invA[i, j] * B[j];
                }
            }
            return X;
        }

        static double[] GaussMethod(double[,] A, double[] B)
        {
            int n = B.Length;
            double[,] AB = new double[n, n + 1];

            // Создаём расширенную матрицу
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    AB[i, j] = A[i, j];
                AB[i, n] = B[i];
            }

            // Прямой ход
            for (int i = 0; i < n; i++)
            {
                // Поиск максимального элемента в столбце
                double max = Math.Abs(AB[i, i]);
                int maxRow = i;
                for (int k = i + 1; k < n; k++)
                {
                    if (Math.Abs(AB[k, i]) > max)
                    {
                        max = Math.Abs(AB[k, i]);
                        maxRow = k;
                    }
                }

                // Перестановка строк
                if (maxRow != i)
                {
                    for (int k = i; k < n + 1; k++)
                    {
                        double temp = AB[i, k];
                        AB[i, k] = AB[maxRow, k];
                        AB[maxRow, k] = temp;
                    }
                }

                // Обнуление элементов ниже главной диагонали
                for (int k = i + 1; k < n; k++)
                {
                    double factor = AB[k, i] / AB[i, i];
                    for (int j = i; j < n + 1; j++)
                    {
                        AB[k, j] -= factor * AB[i, j];
                    }
                }
            }

            // Обратный ход
            double[] X = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                X[i] = AB[i, n];
                for (int j = i + 1; j < n; j++)
                {
                    X[i] -= AB[i, j] * X[j];
                }
                X[i] /= AB[i, i];
            }
            return X;
        }

        static double[] JacobiMethod(double[,] A, double[] B, double eps, int maxIter)
        {
            return JacobiMethodWithDetails(A, B, eps, maxIter, false);
        }

        static double[] JacobiMethodWithDetails(double[,] A, double[] B, double eps, int maxIter, bool printProgress)
        {
            int n = B.Length;
            double[] X = new double[n];
            double[] XNew = new double[n];

            // Начальное приближение (нулевое)
            for (int i = 0; i < n; i++)
                X[i] = 0;

            for (int iter = 0; iter < maxIter; iter++)
            {
                for (int i = 0; i < n; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < n; j++)
                    {
                        if (j != i)
                            sum += A[i, j] * X[j];
                    }
                    XNew[i] = (B[i] - sum) / A[i, i];
                }

                // Вычисление погрешности
                double error = 0;
                for (int i = 0; i < n; i++)
                {
                    error = Math.Max(error, Math.Abs(XNew[i] - X[i]));
                }

                if (printProgress && (iter < 10 || iter % 10 == 9 || error < eps || iter == maxIter - 1))
                {
                    Console.WriteLine($"| {iter + 1,5} | {XNew[0],13:E6} | {XNew[1],13:E6} | {XNew[2],13:E6} | {error,13:E6} |");
                }

                // Проверка условия сходимости
                if (error < eps)
                {
                    if (printProgress)
                        Console.WriteLine($"\n✓ Сходимость достигнута за {iter + 1} итераций");
                    return XNew;
                }

                // Копируем новое приближение
                for (int i = 0; i < n; i++)
                    X[i] = XNew[i];
            }

            if (printProgress)
                Console.WriteLine($"\n⚠️  Достигнут максимум итераций ({maxIter}) без сходимости");
            return XNew;
        }

        static double[] SeidelMethod(double[,] A, double[] B, double eps, int maxIter)
        {
            return SeidelMethodWithDetails(A, B, eps, maxIter, false);
        }

        static double[] SeidelMethodWithDetails(double[,] A, double[] B, double eps, int maxIter, bool printProgress)
        {
            int n = B.Length;
            double[] X = new double[n];

            // Начальное приближение (нулевое)
            for (int i = 0; i < n; i++)
                X[i] = 0;

            for (int iter = 0; iter < maxIter; iter++)
            {
                double[] XOld = (double[])X.Clone();
                double maxError = 0;

                for (int i = 0; i < n; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < n; j++)
                    {
                        if (j != i)
                            sum += A[i, j] * X[j];
                    }
                    X[i] = (B[i] - sum) / A[i, i];
                }

                // Вычисление максимальной погрешности
                for (int i = 0; i < n; i++)
                {
                    double error = Math.Abs(X[i] - XOld[i]);
                    if (error > maxError)
                        maxError = error;
                }

                if (printProgress && (iter < 10 || iter % 10 == 9 || maxError < eps || iter == maxIter - 1))
                {
                    Console.WriteLine($"| {iter + 1,5} | {X[0],13:E6} | {X[1],13:E6} | {X[2],13:E6} | {maxError,13:E6} |");
                }

                if (maxError < eps)
                {
                    if (printProgress)
                        Console.WriteLine($"\n✓ Сходимость достигнута за {iter + 1} итераций");
                    return X;
                }
            }

            if (printProgress)
                Console.WriteLine($"\n⚠️  Достигнут максимум итераций ({maxIter}) без сходимости");
            return X;
        }

        // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ

        static double[,] InverseMatrix(double[,] matrix)
        {
            int n = matrix.GetLength(0);
            double[,] result = new double[n, n];
            double[,] augmented = new double[n, 2 * n];

            // Создаём расширенную матрицу [A|I]
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    augmented[i, j] = matrix[i, j];
                for (int j = n; j < 2 * n; j++)
                    augmented[i, j] = (j - n == i) ? 1.0 : 0.0;
            }

            // Приведение левой части к единичной матрице
            for (int i = 0; i < n; i++)
            {
                // Нормализация строки i
                double pivot = augmented[i, i];
                if (Math.Abs(pivot) < 1e-15)
                    throw new InvalidOperationException("Матрица вырожденная или близка к вырожденной");

                for (int j = 0; j < 2 * n; j++)
                    augmented[i, j] /= pivot;

                // Обнуление других строк
                for (int k = 0; k < n; k++)
                {
                    if (k != i)
                    {
                        double factor = augmented[k, i];
                        for (int j = 0; j < 2 * n; j++)
                            augmented[k, j] -= factor * augmented[i, j];
                    }
                }
            }

            // Извлечение обратной матрицы
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    result[i, j] = augmented[i, j + n];

            return result;
        }

        static double Determinant(double[,] matrix)
        {
            int n = matrix.GetLength(0);
            if (n != matrix.GetLength(1))
                throw new ArgumentException("Матрица должна быть квадратной");

            if (n == 1) return matrix[0, 0];
            if (n == 2) return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];

            double det = 0;
            for (int j = 0; j < n; j++)
            {
                det += (j % 2 == 0 ? 1 : -1) * matrix[0, j] *
                       Determinant(GetMinor(matrix, 0, j));
            }
            return det;
        }

        static double[,] GetMinor(double[,] matrix, int row, int col)
        {
            int n = matrix.GetLength(0);
            double[,] minor = new double[n - 1, n - 1];

            for (int i = 0, mi = 0; i < n; i++)
            {
                if (i == row) continue;
                for (int j = 0, mj = 0; j < n; j++)
                {
                    if (j == col) continue;
                    minor[mi, mj] = matrix[i, j];
                    mj++;
                }
                mi++;
            }
            return minor;
        }

        static void CheckDiagonalDominance(double[,] A)
        {
            int n = A.GetLength(0);
            bool isStrictlyDominant = true;
            bool isDiagonallyDominant = true;

            for (int i = 0; i < n; i++)
            {
                double diagonal = Math.Abs(A[i, i]);
                double sum = 0;

                for (int j = 0; j < n; j++)
                {
                    if (j != i)
                        sum += Math.Abs(A[i, j]);
                }

                if (diagonal > sum)
                {
                    Console.WriteLine($"   Строка {i + 1}: |{A[i, i],6:F2}| > {sum,6:F2} ✓ строгое преобладание");
                }
                else if (diagonal >= sum - 1e-10)
                {
                    Console.WriteLine($"   Строка {i + 1}: |{A[i, i],6:F2}| ≥ {sum,6:F2} нестрогое преобладание");
                    isStrictlyDominant = false;
                }
                else
                {
                    Console.WriteLine($"   Строка {i + 1}: |{A[i, i],6:F2}| < {sum,6:F2} ✗ нет преобладания");
                    isStrictlyDominant = false;
                    isDiagonallyDominant = false;
                }
            }

            if (isStrictlyDominant)
                Console.WriteLine("\n   ✓ Матрица имеет строгое диагональное преобладание");
            else if (isDiagonallyDominant)
                Console.WriteLine("\n   ⚠️  Матрица имеет нестрогое диагональное преобладание");
            else
                Console.WriteLine("\n   ✗ Матрица НЕ имеет диагонального преобладания!");
        }

        static void EstimateConditionNumber(double[,] A)
        {
            // Простая оценка числа обусловленности
            int n = A.GetLength(0);
            double normA = 0;
            double normInvA = 0;

            // Норма матрицы (по строкам)
            for (int i = 0; i < n; i++)
            {
                double rowSum = 0;
                for (int j = 0; j < n; j++)
                {
                    rowSum += Math.Abs(A[i, j]);
                }
                normA = Math.Max(normA, rowSum);
            }

            try
            {
                double[,] invA = InverseMatrix(A);

                // Норма обратной матрицы
                for (int i = 0; i < n; i++)
                {
                    double rowSum = 0;
                    for (int j = 0; j < n; j++)
                    {
                        rowSum += Math.Abs(invA[i, j]);
                    }
                    normInvA = Math.Max(normInvA, rowSum);
                }

                double cond = normA * normInvA;
                Console.WriteLine($"   Число обусловленности cond(A) ≈ {cond:E2}");

                if (cond < 1e3)
                    Console.WriteLine("   ✓ Хорошо обусловленная система");
                else if (cond < 1e6)
                    Console.WriteLine("   ⚠️  Умеренно плохо обусловленная система");
                else
                    Console.WriteLine("   ✗ Плохо обусловленная система!");
            }
            catch
            {
                Console.WriteLine("   Не удалось оценить число обусловленности");
            }
        }

        static void CheckSymmetry(double[,] A)
        {
            int n = A.GetLength(0);
            bool symmetric = true;

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (Math.Abs(A[i, j] - A[j, i]) > 1e-10)
                    {
                        symmetric = false;
                        break;
                    }
                }
                if (!symmetric) break;
            }

            if (symmetric)
                Console.WriteLine("   ✓ Матрица симметричная");
            else
                Console.WriteLine("   ✗ Матрица несимметричная");
        }

        static void PrintSolutionTable(double[] X, string methodName)
        {
            Console.WriteLine($"\nРЕШЕНИЕ ({methodName}):");
            Console.WriteLine(new string('=', 50));
            Console.WriteLine($"x₁ = {X[0],15:F10}");
            Console.WriteLine($"x₂ = {X[1],15:F10}");
            Console.WriteLine($"x₃ = {X[2],15:F10}");
            Console.WriteLine(new string('=', 50));
        }

        static void CheckSolution(double[,] A, double[] B, double[] X, string methodName)
        {
            Console.WriteLine($"\nПРОВЕРКА РЕШЕНИЯ ({methodName}):");
            Console.WriteLine(new string('-', 60));

            double[] residuals = new double[B.Length];
            double maxResidual = 0;

            for (int i = 0; i < B.Length; i++)
            {
                double sum = 0;
                for (int j = 0; j < B.Length; j++)
                {
                    sum += A[i, j] * X[j];
                }
                residuals[i] = B[i] - sum;
                maxResidual = Math.Max(maxResidual, Math.Abs(residuals[i]));

                Console.WriteLine($"Ур. {i + 1}: {B[i],6:F3} - ({sum,10:F6}) = {residuals[i],12:E6}");
            }

            Console.WriteLine(new string('-', 60));
            Console.WriteLine($"Максимальная невязка: {maxResidual:E6}");

            if (maxResidual < 1e-6)
                Console.WriteLine("✓ Решение точное (невязка < 1e-6)");
            else if (maxResidual < 1e-3)
                Console.WriteLine("✓ Решение хорошее (невязка < 1e-3)");
            else if (maxResidual < 0.1)
                Console.WriteLine("⚠️  Решение удовлетворительное (невязка < 0.1)");
            else
                Console.WriteLine("✗ Решение неточное (невязка ≥ 0.1)");
        }

        static void PrintComparisonRow(string methodName, double[] X, double[,] A, double[] B)
        {
            // Вычисляем значения левых частей уравнений
            double[] LHS = new double[3];
            for (int i = 0; i < 3; i++)
            {
                LHS[i] = 0;
                for (int j = 0; j < 3; j++)
                {
                    LHS[i] += A[i, j] * X[j];
                }
            }

            Console.WriteLine($"| {methodName,-12} | {X[0],14:F8} | {X[1],14:F8} | {X[2],14:F8} |" +
                              $" {Math.Abs(B[0] - LHS[0]),13:E4} | {Math.Abs(B[1] - LHS[1]),13:E4} | {Math.Abs(B[2] - LHS[2]),13:E4} |");
        }

        static void CompareWithReference(double[] reference, double[] solution, string methodName)
        {
            double maxDiff = 0;
            for (int i = 0; i < reference.Length; i++)
            {
                maxDiff = Math.Max(maxDiff, Math.Abs(reference[i] - solution[i]));
            }

            Console.Write($"   {methodName,-8}: максимальное отличие = {maxDiff:E6}");

            if (maxDiff < 1e-6)
                Console.WriteLine(" ✓ отличное совпадение");
            else if (maxDiff < 1e-3)
                Console.WriteLine(" ✓ хорошее совпадение");
            else if (maxDiff < 0.1)
                Console.WriteLine(" ✓ удовлетворительное");
            else
                Console.WriteLine(" ✗ значительное расхождение");
        }
    }
}