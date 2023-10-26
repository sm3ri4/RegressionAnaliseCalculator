using System;
using System.Web.UI.DataVisualization.Charting;

namespace ProbabilityTheoryCourse{
    class Student{

        private double _def;
        private int _degreeOfFreedom;
        private double _level;

        public Student(double def, int count, double level){
            _def = def;
            _degreeOfFreedom = count - 2;
            _level = level;
        }   

        public double GetPairedStudentCoeff{
            get{
                return Math.Sqrt(_def * _def * _degreeOfFreedom / (1 - _def * _def));
            }
        }

        public double GetCriticalPairedStudentCoeff{
            get{
                Chart chart = new Chart();
                return chart.DataManipulator.Statistics.InverseTDistribution(1 - _level, _degreeOfFreedom);
            }
        }

        public double GetPartialStudentCoeff{
            get{
                return Math.Sqrt(_def * _def * (_degreeOfFreedom + 2) / (1 - _def * _def));
            }
        }

        public double GetCriticalPartialStudentCoeff{
            get{
                Chart chart = new Chart();
                return chart.DataManipulator.Statistics.InverseTDistribution(1 - _level, _degreeOfFreedom + 2);
            }
        }

    }
}
