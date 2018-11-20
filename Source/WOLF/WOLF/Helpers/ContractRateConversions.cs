using System.Collections.Generic;

namespace WOLF
{
    public static class ContractRateConversions
    {
        private static Dictionary<ContractRateUnit, Dictionary<ContractRateUnit, double>> _rateConversions
            = new Dictionary<ContractRateUnit, Dictionary<ContractRateUnit, double>>
            {
                {
                    ContractRateUnit.PerDay,
                    new Dictionary<ContractRateUnit, double>
                    {
                        { ContractRateUnit.PerDay, 1d },
                        { ContractRateUnit.PerHour, 1d / 6d },
                        { ContractRateUnit.PerMinute, 1d / 6d / 60d },
                        { ContractRateUnit.PerSecond, 1d / 6d / 60d / 60d }
                    }
                },
                {
                    ContractRateUnit.PerHour,
                    new Dictionary<ContractRateUnit, double>
                    {
                        { ContractRateUnit.PerDay, 6d },
                        { ContractRateUnit.PerHour, 1d },
                        { ContractRateUnit.PerMinute, 1d / 60d },
                        { ContractRateUnit.PerSecond, 1d / 60d / 60d }
                    }
                },
                {
                    ContractRateUnit.PerMinute,
                    new Dictionary<ContractRateUnit, double>
                    {
                        { ContractRateUnit.PerDay, 6d * 60d },
                        { ContractRateUnit.PerHour, 60d },
                        { ContractRateUnit.PerMinute, 1d },
                        { ContractRateUnit.PerSecond, 1d / 60d }
                    }
                },
                {
                    ContractRateUnit.PerSecond,
                    new Dictionary<ContractRateUnit, double>
                    {
                        { ContractRateUnit.PerDay, 6d * 60d * 60d },
                        { ContractRateUnit.PerHour, 60d * 60d },
                        { ContractRateUnit.PerMinute, 60d },
                        { ContractRateUnit.PerSecond, 1d }
                    }
                },
            };

        public static double Convert(double quantity, ContractRateUnit fromRate, ContractRateUnit toRate)
        {
            return quantity * _rateConversions[fromRate][toRate];
        }
    }
}
