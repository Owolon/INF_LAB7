using System;
using System.Collections.Generic;

namespace NumericalDE
{
    class Program
    {
        // Параметры варианта 16 (1, 6, 7, 7)
        static readonly double h0 = 0.5;
        static readonly double x0 = -1.0;
        static readonly double b = 2 * Math.PI - 1; // ≈ 5.283185307179586
        static readonly double y0 = 8.0;
        static readonly double epsilon = 1e-3; // Заданная точность

        // Функции по варианту
        static double g(double x) => -Math.Sin(x + 1);

        static double phi(double x) => Math.Exp(-x * x) * Math.Cos(0.8 * x);

        static double psi(double x) => Math.Exp(3 * x - 3);

        static double F(double x) => phi(x) * psi(x);

        static double f(double x, double y) => F(x) - g(x) * y;

        // Явный метод Эйлера (один шаг)
        static double EulerStep(double x, double y, double h) => y + h * f(x, y);

        // Решение с автоматическим выбором шага
        static (List<double> x, List<double> y, List<double> h) SolveAutoStep(double eps)
        {
            var xValues = new List<double> { x0 };
            var yValues = new List<double> { y0 };
            var hValues = new List<double> { h0 };

            double xCurr = x0;
            double yCurr = y0;
            double hCurr = h0;
            const int r = 1; // Порядок метода Эйлера

            while (xCurr < b - 1e-10)
            {
                // Корректировка последнего шага
                if (xCurr + hCurr > b)
                    hCurr = b - xCurr;

                bool stepAccepted = false;

                while (!stepAccepted)
                {
                    // Шаг h
                    double y1 = EulerStep(xCurr, yCurr, hCurr);

                    // Два шага h/2
                    double yTemp = EulerStep(xCurr, yCurr, hCurr / 2);
                    double y2 = EulerStep(xCurr + hCurr / 2, yTemp, hCurr / 2);

                    // Оценка погрешности
                    double delta = Math.Abs(y1 - y2) / (Math.Pow(2, r) - 1);

                    if (delta >= eps)
                    {
                        // Точность не достигнута - уменьшаем шаг
                        hCurr /= 2;
                    }
                    else
                    {
                        // Точность достигнута
                        double yNext = y1;
                        double xNext = xCurr + hCurr;

                        // Сохраняем результаты
                        xValues.Add(xNext);
                        yValues.Add(yNext);
                        hValues.Add(hCurr);

                        // Планирование следующего шага
                        if (delta < eps / Math.Pow(2, r + 1))
                            hCurr *= 2; // Можно увеличить шаг

                        // Переходим к следующей точке
                        xCurr = xNext;
                        yCurr = yNext;
                        stepAccepted = true;
                    }
                }
            }

            return (xValues, yValues, hValues);
        }

        // Решение с постоянным шагом
        static (List<double> x, List<double> y) SolveConstantStep(int N)
        {
            double h = (b - x0) / N;
            var xVals = new List<double> { x0 };
            var yVals = new List<double> { y0 };

            double xCurr = x0;
            double yCurr = y0;

            for (int i = 0; i < N; i++)
            {
                double yNext = EulerStep(xCurr, yCurr, h);
                double xNext = xCurr + h;

                xVals.Add(xNext);
                yVals.Add(yNext);

                xCurr = xNext;
                yCurr = yNext;
            }

            return (xVals, yVals);
        }

        // Поиск ближайшего значения в отсортированном массиве
        static double FindClosestY(List<double> xList, List<double> yList, double targetX)
        {
            // Ищем ближайшую точку
            int closestIndex = 0;
            double minDiff = Math.Abs(xList[0] - targetX);

            for (int i = 1; i < xList.Count; i++)
            {
                double diff = Math.Abs(xList[i] - targetX);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    closestIndex = i;
                }
            }

            return yList[closestIndex];
        }

        static void Main()
        {
            Console.WriteLine(new string('=', 70));
            Console.WriteLine("РЕШЕНИЕ ЗАДАЧИ КОШИ");
            Console.WriteLine($"Уравнение: y' = e^(-x²+3x-3)*cos(0.8x) + sin(x+1)*y");
            Console.WriteLine($"Интервал: [{x0}, {b:F6}], y({x0}) = {y0}");
            Console.WriteLine($"Точность ε = {epsilon}");
            Console.WriteLine(new string('=', 70));

            // А) С автоматическим выбором шага
            Console.WriteLine("\nА) РЕШЕНИЕ С АВТОМАТИЧЕСКИМ ВЫБОРОМ ШАГА:");
            var (xAuto, yAuto, hAuto) = SolveAutoStep(epsilon);
            int N = xAuto.Count - 1;

            Console.WriteLine($"Количество шагов: {N}");
            Console.WriteLine($"Последний шаг: {hAuto[^1]:F6}");
            Console.WriteLine($"y({xAuto[^1]:F6}) = {yAuto[^1]:F10}");

            // Б) С постоянным шагом
            Console.WriteLine($"\nБ) РЕШЕНИЕ С ПОСТОЯННЫМ ШАГОМ:");
            Console.WriteLine($"(h = (b-x0)/N, где N = {N} из пункта А)");
            var (xConst, yConst) = SolveConstantStep(N);
            double hConst = (b - x0) / N;
            Console.WriteLine($"Постоянный шаг h = {hConst:F6}");
            Console.WriteLine($"y({xConst[^1]:F6}) = {yConst[^1]:F10}");

            // Сравнительная таблица
            Console.WriteLine("\n" + new string('=', 70));
            Console.WriteLine("СРАВНИТЕЛЬНАЯ ТАБЛИЦА РЕЗУЛЬТАТОВ");
            Console.WriteLine(new string('=', 70));
            Console.WriteLine("{0,4} | {1,12} | {2,18} | {3,18} | {4,12}",
                "№", "x", "y_auto(x)", "y_const(x)", "Разность");
            Console.WriteLine(new string('-', 75));

            // Вывод таблицы (каждой N-й точки для компактности)
            int step = Math.Max(1, N / 20);

            for (int i = 0; i <= N; i++)
            {
                if (i == 0 || i == N || i % step == 0)
                {
                    double x = xAuto[i];
                    double yA = yAuto[i];
                    double yC = FindClosestY(xConst, yConst, x);
                    double diff = Math.Abs(yA - yC);

                    string stepInfo = i < hAuto.Count ? $"[h={hAuto[i]:F4}]" : "";
                    Console.WriteLine("{0,4} | {1,12:F6} | {2,18:F10} | {3,18:F10} | {4,12:E2} {5}",
                        i, x, yA, yC, diff, stepInfo);
                }
            }

            // Итоговое сравнение
            Console.WriteLine(new string('-', 75));
            double finalDiff = Math.Abs(yAuto[^1] - yConst[^1]);
            Console.WriteLine($"ИТОГО: | Шагов: {N,8} | y(b)={yAuto[^1]:F8} | y(b)={yConst[^1]:F8} | diff={finalDiff:E2}");
            Console.WriteLine(new string('=', 70));

            // Дополнительная информация
            Console.WriteLine("\nДОПОЛНИТЕЛЬНАЯ ИНФОРМАЦИЯ:");

            double minH = double.MaxValue, maxH = double.MinValue, sumH = 0;
            for (int i = 0; i < hAuto.Count; i++)
            {
                if (hAuto[i] < minH) minH = hAuto[i];
                if (hAuto[i] > maxH) maxH = hAuto[i];
                sumH += hAuto[i];
            }

            Console.WriteLine($"1. Минимальный шаг (авто): {minH:F6}");
            Console.WriteLine($"2. Максимальный шаг (авто): {maxH:F6}");
            Console.WriteLine($"3. Средний шаг (авто): {sumH / hAuto.Count:F6}");
            Console.WriteLine($"4. Теоретический постоянный шаг: {hConst:F6}");

            // Анализ эффективности
            Console.WriteLine($"\nАНАЛИЗ ЭФФЕКТИВНОСТИ:");
            Console.WriteLine($"Автоматический шаг: использовано {N} шагов");
            Console.WriteLine($"Постоянный шаг: использовано {N} шагов");
            Console.WriteLine($"Максимальное отклонение: {finalDiff:E2}");

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}