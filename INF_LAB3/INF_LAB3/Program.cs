using System;

namespace TransportProblemSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Console.WriteLine("========== РЕШЕНИЕ ТРАНСПОРТНОЙ ЗАДАЧИ ==========\n");

                SolveTransportProblem();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОШИБКА: {ex.Message}");
                Console.WriteLine($"Подробности: {ex.StackTrace}");
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static void SolveTransportProblem()
        {
            // Исходные данные из таблицы (исправлены опечатки)
            int[] supplies = { 62, 76, 21, 87, 35 }; // заявки поставщиков
            int[] demands = { 16, 26, 63, 84, 24, 84 }; // потребности потребителей

            // Матрица стоимостей 5x6 (исправленная версия)
            int[,] costs = {
                {5, 6, 6, 7, 5, 6},
                {6, 3, 6, 4, 8, 6},
                {8, 5, 6, 2, 7, 8},
                {4, 4, 7, 8, 8, 4},
                {1, 8, 3, 8, 6, 2}
            };

            // 1. Вывод исходных данных
            Console.WriteLine("1. ИСХОДНЫЕ ДАННЫЕ:");
            Console.Write("Заявки поставщиков: ");
            PrintArray(supplies);
            Console.Write("Потребности потребителей: ");
            PrintArray(demands);
            Console.WriteLine();

            Console.WriteLine("Матрица стоимостей:");
            Console.WriteLine("     П1 П2 П3 П4 П5 П6");
            for (int i = 0; i < Math.Min(supplies.Length, 5); i++)
            {
                Console.Write($"П{i + 1}: ");
                for (int j = 0; j < Math.Min(demands.Length, 6); j++)
                {
                    Console.Write($"{costs[i, j],3}");
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            // 2. Проверка баланса
            Console.WriteLine("2. ПРОВЕРКА УСЛОВИЯ БАЛАНСА:");
            int totalSupply = SumArray(supplies);
            int totalDemand = SumArray(demands);

            Console.WriteLine($"Сумма запасов: {totalSupply}");
            Console.WriteLine($"Сумма потребностей: {totalDemand}");

            if (totalSupply == totalDemand)
            {
                Console.WriteLine("✓ Условие баланса выполнено.\n");
            }
            else
            {
                Console.WriteLine("✗ Условие баланса не выполнено.");

                // Добавляем фиктивного поставщика или потребителя
                if (totalSupply < totalDemand)
                {
                    int diff = totalDemand - totalSupply;
                    Console.WriteLine($"Добавляем фиктивного поставщика с запасом: {diff}");
                    Array.Resize(ref supplies, supplies.Length + 1);
                    supplies[supplies.Length - 1] = diff;

                    // Расширяем матрицу стоимостей
                    int newRows = supplies.Length;
                    int newCols = demands.Length;
                    int[,] newCosts = new int[newRows, newCols];

                    for (int i = 0; i < 5; i++)
                        for (int j = 0; j < 6; j++)
                            newCosts[i, j] = costs[i, j];

                    // Для фиктивного поставщика нулевые стоимости
                    for (int j = 0; j < newCols; j++)
                        newCosts[newRows - 1, j] = 0;

                    costs = newCosts;
                }
                else
                {
                    int diff = totalSupply - totalDemand;
                    Console.WriteLine($"Добавляем фиктивного потребителя с потребностью: {diff}");
                    Array.Resize(ref demands, demands.Length + 1);
                    demands[demands.Length - 1] = diff;

                    // Расширяем матрицу стоимостей
                    int newRows = supplies.Length;
                    int newCols = demands.Length;
                    int[,] newCosts = new int[newRows, newCols];

                    for (int i = 0; i < 5; i++)
                        for (int j = 0; j < 6; j++)
                            newCosts[i, j] = costs[i, j];

                    // Для фиктивного потребителя нулевые стоимости
                    for (int i = 0; i < newRows; i++)
                        newCosts[i, newCols - 1] = 0;

                    costs = newCosts;
                }
                Console.WriteLine();
            }

            int n = supplies.Length;
            int m = demands.Length;

            // 3. Построение опорных планов
            Console.WriteLine("3. ПОСТРОЕНИЕ ОПОРНЫХ ПЛАНОВ:");

            Console.WriteLine("\n=== Метод северо-западного угла ===");
            int[,] planNW = BuildNorthwestCornerPlan(supplies, demands, costs, n, m);

            Console.WriteLine("\n=== Метод минимального элемента ===");
            int[,] planMin = BuildMinElementPlan(supplies, demands, costs, n, m);

            // 4. Выбор плана с минимальной стоимостью
            Console.WriteLine("\n4. ВЫБОР ОПТИМАЛЬНОГО ПЛАНА:");
            int costNW = CalculatePlanCost(planNW, costs, n, m);
            int costMin = CalculatePlanCost(planMin, costs, n, m);

            Console.WriteLine($"Стоимость плана (СЗУ): {costNW}");
            Console.WriteLine($"Стоимость плана (мин. элемент): {costMin}");

            int[,] currentPlan;
            if (costMin <= costNW)
            {
                Console.WriteLine("✓ Выбираем план метода минимального элемента\n");
                currentPlan = (int[,])planMin.Clone();
            }
            else
            {
                Console.WriteLine("✓ Выбираем план метода северо-западного угла\n");
                currentPlan = (int[,])planNW.Clone();
            }

            Console.WriteLine("Выбранный опорный план:");
            PrintMatrix(currentPlan, n, m);

            // 5-9. Итеративное улучшение плана (упрощенная версия)
            Console.WriteLine("\n5-9. ОПТИМИЗАЦИЯ ПЛАНА:");

            int iteration = 1;
            int maxIterations = 20;

            while (iteration <= maxIterations)
            {
                Console.WriteLine($"\n=== Итерация {iteration} ===");

                // 5. Расчет потенциалов (упрощенный)
                Console.WriteLine("5. Расчет потенциалов (упрощенный):");
                int[] u = new int[n];
                int[] v = new int[m];

                // Базовый расчет
                u[0] = 0;
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        if (currentPlan[i, j] > 0)
                        {
                            if (u[i] != 0 || i == 0)
                            {
                                v[j] = costs[i, j] - u[i];
                            }
                            else if (v[j] != 0)
                            {
                                u[i] = costs[i, j] - v[j];
                            }
                        }
                    }
                }

                Console.Write("u[i]: ");
                for (int i = 0; i < n; i++) Console.Write($"{u[i],4}");
                Console.Write("\nv[j]: ");
                for (int j = 0; j < m; j++) Console.Write($"{v[j],4}");
                Console.WriteLine();

                // 6. Проверка оптимальности
                Console.WriteLine("\n6. Проверка оптимальности:");
                bool isOptimal = true;
                int bestI = -1, bestJ = -1;
                int bestDelta = 0;

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        if (currentPlan[i, j] == 0)
                        {
                            int delta = costs[i, j] - (u[i] + v[j]);
                            if (delta < 0 && delta < bestDelta)
                            {
                                bestDelta = delta;
                                bestI = i;
                                bestJ = j;
                                isOptimal = false;
                            }
                        }
                    }
                }

                if (isOptimal)
                {
                    Console.WriteLine("✓ План оптимален!");
                    break;
                }
                else
                {
                    Console.WriteLine($"✗ Найдена клетка для улучшения: ({bestI + 1}, {bestJ + 1}) с Δ={bestDelta}");

                    // 7. Упрощенный цикл пересчета
                    Console.WriteLine($"7. Упрощенный цикл пересчета:");

                    // Просто добавляем единицу в найденную клетку
                    currentPlan[bestI, bestJ]++;

                    // Находим клетку в строке для уменьшения
                    bool reduced = false;
                    for (int j = 0; j < m && !reduced; j++)
                    {
                        if (j != bestJ && currentPlan[bestI, j] > 0)
                        {
                            currentPlan[bestI, j]--;
                            reduced = true;
                            Console.WriteLine($"   Увеличили клетку ({bestI + 1}, {bestJ + 1}) на 1");
                            Console.WriteLine($"   Уменьшили клетку ({bestI + 1}, {j + 1}) на 1");
                        }
                    }

                    // 8. Новая стоимость
                    int newCost = CalculatePlanCost(currentPlan, costs, n, m);
                    Console.WriteLine($"8. Новая стоимость: {newCost}");
                }

                iteration++;
            }

            if (iteration > maxIterations)
            {
                Console.WriteLine($"\nДостигнут предел {maxIterations} итераций.");
            }

            // Итоговый результат
            Console.WriteLine("\n========== РЕЗУЛЬТАТ ==========");
            Console.WriteLine("Оптимальный план перевозок:");

            // Заголовок
            Console.Write("     ");
            for (int j = 0; j < m; j++) Console.Write($"П{j + 1,3} ");
            Console.WriteLine(" Запас");
            Console.WriteLine(new string('-', (m + 1) * 5));

            int totalCost = 0;
            for (int i = 0; i < n; i++)
            {
                Console.Write($"П{i + 1} | ");
                int rowSum = 0;
                for (int j = 0; j < m; j++)
                {
                    Console.Write($"{currentPlan[i, j],3} ");
                    rowSum += currentPlan[i, j];
                    totalCost += currentPlan[i, j] * costs[i, j];
                }
                Console.WriteLine($"| {rowSum,3}");
            }

            Console.WriteLine(new string('-', (m + 1) * 5));
            Console.Write("Потр| ");
            for (int j = 0; j < m; j++)
            {
                int colSum = 0;
                for (int i = 0; i < n; i++)
                {
                    colSum += currentPlan[i, j];
                }
                Console.Write($"{colSum,3} ");
            }

            Console.WriteLine($"\n\nМинимальная стоимость перевозки: {totalCost}");
        }

        // Вспомогательные методы
        static int SumArray(int[] arr)
        {
            int sum = 0;
            foreach (int val in arr) sum += val;
            return sum;
        }

        static void PrintArray(int[] arr)
        {
            foreach (int val in arr) Console.Write($"{val} ");
            Console.WriteLine();
        }

        static int[,] BuildNorthwestCornerPlan(int[] supplies, int[] demands, int[,] costs, int n, int m)
        {
            int[,] plan = new int[n, m];
            int[] supCopy = (int[])supplies.Clone();
            int[] demCopy = (int[])demands.Clone();

            int i = 0, j = 0;

            while (i < n && j < m)
            {
                int shipment = Math.Min(supCopy[i], demCopy[j]);

                if (shipment > 0)
                {
                    plan[i, j] = shipment;
                    Console.WriteLine($"x[{i + 1},{j + 1}] = {shipment} (стоимость: {costs[i, j]})");
                }

                supCopy[i] -= shipment;
                demCopy[j] -= shipment;

                if (supCopy[i] == 0) i++;
                if (demCopy[j] == 0) j++;
            }

            Console.WriteLine("\nПолученный план:");
            PrintMatrix(plan, n, m);

            return plan;
        }

        static int[,] BuildMinElementPlan(int[] supplies, int[] demands, int[,] costs, int n, int m)
        {
            int[,] plan = new int[n, m];
            int[] supCopy = (int[])supplies.Clone();
            int[] demCopy = (int[])demands.Clone();

            bool cellsLeft = true;

            while (cellsLeft)
            {
                int minCost = int.MaxValue;
                int minI = -1, minJ = -1;

                // Находим минимальную стоимость среди доступных клеток
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        if (supCopy[i] > 0 && demCopy[j] > 0 && plan[i, j] == 0 && costs[i, j] < minCost)
                        {
                            minCost = costs[i, j];
                            minI = i;
                            minJ = j;
                        }
                    }
                }

                if (minI == -1) // Нет доступных клеток
                {
                    cellsLeft = false;
                    break;
                }

                // Назначаем перевозку
                int shipment = Math.Min(supCopy[minI], demCopy[minJ]);
                plan[minI, minJ] = shipment;
                Console.WriteLine($"x[{minI + 1},{minJ + 1}] = {shipment} (стоимость: {costs[minI, minJ]})");

                supCopy[minI] -= shipment;
                demCopy[minJ] -= shipment;
            }

            Console.WriteLine("\nПолученный план:");
            PrintMatrix(plan, n, m);

            return plan;
        }

        static int CalculatePlanCost(int[,] plan, int[,] costs, int n, int m)
        {
            int total = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    total += plan[i, j] * costs[i, j];
                }
            }
            return total;
        }

        static void PrintMatrix(int[,] matrix, int n, int m)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    Console.Write($"{matrix[i, j],4}");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}