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
            DateTime thisDay = DateTime.Today;
            DateTime thisday5 = thisDay.AddDays(5.0);
            Datepick.Text = thisday5.ToString();
            System.Net.WebClient wc = new System.Net.WebClient();
            string stockname = Stockname.Text; 
            byte[] raw = wc.DownloadData("https://api.iextrading.com/1.0/stock/" + stockname + "/price");
            string webData = System.Text.Encoding.UTF8.GetString(raw);
            Stockprice.Text = webData;
            double guessstrike = Math.Round(Convert.ToDouble(webData), 0);
            Strikeprice.Text = guessstrike.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            Option.Type optionType;
            if (CallorPut.Text == "Call")
                {  optionType = Option.Type.Call;
            }
            else
            {
                optionType = Option.Type.Put;
            }

                
            double underlyingPrice = Convert.ToDouble(Stockprice.Text);
            double strikePrice = Convert.ToDouble(Strikeprice.Text);
            double dividendYield = 0.0;
            double riskFreeRate = Convert.ToDouble(Intrate.Text);
            double volatility = Convert.ToDouble(Resultvol.Text)/100;
            Date todaydate = Date.todaysDate();
            string expd = Datepick.Text;
            Date maturityDate = new Date();
            if (expd[1].ToString() is "/")
            {
                expd = '0' + expd;
            }
            if (expd[4].ToString() is "/")
            {
                expd = expd.Substring(0,3) + '0' + expd.Substring(3);
            }
            maturityDate = DateParser.parseFormatted(expd, "%m/%d/%Y");

            




            Settings.instance().setEvaluationDate(todaydate);

            Date settlementDate = new Date();
            settlementDate = todaydate;

            

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

            //americanOption.setPricingEngine(
            //                 new BaroneAdesiWhaleyEngine(stochasticProcess));

            //americanOption2.setPricingEngine(
            //                 new BinomialVanillaEngine(stochasticProcess, "coxrossrubinstein",1000));

            europeanOption.setPricingEngine(
                              new AnalyticEuropeanEngine(stochasticProcess));

            
            
            
            
            Date divdate1 = new Date(14, Month.December, 2019);
            DoubleVector divpay = new DoubleVector();
            DateVector divDates = new DateVector();
            //divpay.Add(.0001);
            //divDates.Add(divdate1);
            DividendVanillaOption americanOption1 = new DividendVanillaOption(payoff, americanExercise, divDates, divpay);
            
            FDDividendAmericanEngine engine = new FDDividendAmericanEngine(stochasticProcess);
            americanOption1.setPricingEngine(engine);
            double opprice4 = americanOption1.NPV();
            //double vol1 = americanOption1.impliedVolatility(opprice4, stochasticProcess, .001);
            double delta1 = Math.Round(americanOption1.delta(),2);
            double gamma1 = Math.Round(americanOption1.gamma(),2);
            double theta1 = Math.Round(europeanOption.theta()/365,2);
            double vega1 = Math.Round(europeanOption.vega()/100,2);
            double oppricedisplay = Math.Round(opprice4, 3);
            Resultam.Text = oppricedisplay.ToString();
            Resultam_Delta.Text = delta1.ToString();
            Resultam_Gamma.Text = gamma1.ToString();
            Resultam_Theta.Text = theta1.ToString();
            Resultam_Vega.Text = vega1.ToString();




        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Option.Type optionType;
            if (CallorPut.Text == "Call")
            {
                optionType = Option.Type.Call;
            }
            else
            {
                optionType = Option.Type.Put;
            }


            double underlyingPrice = Convert.ToDouble(Stockprice.Text);
            double strikePrice = Convert.ToDouble(Strikeprice.Text);
            double dividendYield = 0.0;
            double riskFreeRate = Convert.ToDouble(Intrate.Text);
            double volatility = Convert.ToDouble(Resultvol.Text) / 100;
            Date todaydate = Date.todaysDate();
            string expd = Datepick.Text;
            Date maturityDate = new Date();
            if (expd[1].ToString()  is "/"){
                expd = '0' + expd; }
            if (expd[4].ToString() is "/")
            {
                expd = expd.Substring(0, 3) + '0' + expd.Substring(3);
            }
            maturityDate = DateParser.parseFormatted(expd, "%m/%d/%Y");






            Settings.instance().setEvaluationDate(todaydate);

            Date settlementDate = new Date();
            settlementDate = todaydate;



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
                             new BinomialVanillaEngine(stochasticProcess, "coxrossrubinstein", 1000));

            europeanOption.setPricingEngine(
                              new AnalyticEuropeanEngine(stochasticProcess));

            //double opprice = Math.Round(americanOption2.NPV(), 3);



            Date divdate1 = new Date(14, Month.January, 2019);
            DoubleVector divpay = new DoubleVector();
            DateVector divDates = new DateVector();
            //divpay.Add(.0001);
            //divDates.Add(divdate1);
            DividendVanillaOption americanOption1 = new DividendVanillaOption(payoff, americanExercise, divDates, divpay);

            FDDividendAmericanEngine engine = new FDDividendAmericanEngine(stochasticProcess);
            americanOption1.setPricingEngine(engine);
            //double opprice4 = americanOption1.NPV();
            double Inoprice  = Convert.ToDouble(Resultam.Text);
            double vol1 = americanOption1.impliedVolatility(Inoprice, stochasticProcess, .0001);
            vol1 = Math.Round(vol1, 4) * 100;
            double delta1 = Math.Round(americanOption2.delta(), 2);
            double gamma1 = Math.Round(americanOption2.gamma(), 2);
            double theta1 = Math.Round(europeanOption.theta() / 365, 2);
            double vega1 = Math.Round(europeanOption.vega() / 100, 2);

            //Resultam.Text = opprice4.ToString();
            Resultam_Delta.Text = delta1.ToString();
            Resultam_Gamma.Text = gamma1.ToString();
            Resultam_Theta.Text = theta1.ToString();
            Resultam_Vega.Text = vega1.ToString();
            Resultvol.Text = vol1.ToString();
        }

        

        private void Stockname_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        

        

        private void KeyDownE(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                string stockname = Stockname.Text;
                byte[] raw = wc.DownloadData("https://api.iextrading.com/1.0/stock/" + stockname + "/price");
                string webData = System.Text.Encoding.UTF8.GetString(raw);
                Stockprice.Text = webData;
            }
        }
    }
}
