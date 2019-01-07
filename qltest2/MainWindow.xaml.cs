using QuantLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace qltest2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Option.Type optionType = Option.Type.Call;
            double underlyingPrice = 155;
            double strikePrice = 155;
            double dividendYield = 0.0;
            double riskFreeRate = 0.02;
            double volatility = Convert.ToDouble(Resultvol.Text)/100;
            Date todaydate = Date.todaysDate();
            
            Settings.instance().setEvaluationDate(todaydate);

            Date settlementDate = new Date(7, Month.January, 2019);
            Date maturityDate = new Date(11, Month.January, 2019);

            QuantLib.Calendar calendar = new TARGET();

            AmericanExercise americanExercise =
                new AmericanExercise(settlementDate, maturityDate);

            EuropeanExercise europeanExercise =
                new EuropeanExercise(maturityDate);

            DayCounter dayCounter = new Actual365Fixed();
            YieldTermStructureHandle flatRateTSH =
                new YieldTermStructureHandle(
                                new FlatForward(settlementDate, riskFreeRate,
                                                 dayCounter));
            YieldTermStructureHandle flatDividendTSH =
                new YieldTermStructureHandle(
                                new FlatForward(settlementDate, dividendYield,
                                                dayCounter));
            BlackVolTermStructureHandle flatVolTSH =
                new BlackVolTermStructureHandle(
                                new BlackConstantVol(settlementDate, calendar,
                                                     volatility, dayCounter));

            QuoteHandle underlyingQuoteH =
                new QuoteHandle(new SimpleQuote(underlyingPrice));

            BlackScholesMertonProcess stochasticProcess =
                new BlackScholesMertonProcess(underlyingQuoteH,
                                              flatDividendTSH,
                                              flatRateTSH,
                                              flatVolTSH);

            PlainVanillaPayoff payoff =
                new PlainVanillaPayoff(optionType, strikePrice);

            VanillaOption americanOption =
                new VanillaOption(payoff, americanExercise);

            VanillaOption americanOption2 =
                new VanillaOption(payoff, americanExercise);

            VanillaOption europeanOption =
                new VanillaOption(payoff, europeanExercise);

            americanOption.setPricingEngine(
                             new BaroneAdesiWhaleyEngine(stochasticProcess));

            americanOption2.setPricingEngine(
                             new BinomialVanillaEngine(stochasticProcess, "coxrossrubinstein",1000));

            europeanOption.setPricingEngine(
                              new AnalyticEuropeanEngine(stochasticProcess));

            double opprice = americanOption2.NPV();
            
            
            
            Date divdate1 = new Date(10, Month.June, 1998);
            DoubleVector divpay = new DoubleVector();
            DateVector divDates = new DateVector();
            divpay.Add(.6);
            divDates.Add(divdate1);
            DividendVanillaOption americanOption1 = new DividendVanillaOption(payoff, americanExercise, divDates, divpay);
            
            FDDividendAmericanEngine engine = new FDDividendAmericanEngine(stochasticProcess);
            americanOption1.setPricingEngine(engine);
            //double opprice = americanOption1.NPV();
            double delta1 = americanOption2.delta();
            double gamma1 = americanOption2.gamma();
            double theta1 = europeanOption.theta()/365;
            double vega1 = europeanOption.vega();

            Resultam.Text = opprice.ToString();
            Resultam_Delta.Text = delta1.ToString();
            Resultam_Gamma.Text = gamma1.ToString();
            Resultam_Theta.Text = theta1.ToString();
            Resultam_Vega.Text = vega1.ToString();




        }
    }
}
