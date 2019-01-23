package A4;

import weka.core.Instances;

public class App 
{
	public static final String DATAFILE="spiral.arff";

    public static void main( String[] args )
    {
        //System.out.println( "Hello World!" );
    	LogisticRegressionDemo test = new LogisticRegressionDemo();
    	Instances data;
		try {
			data = test.getDataSet(DATAFILE);
			test.startLogistic(data);
			test.startMP(data);
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
    }
}
