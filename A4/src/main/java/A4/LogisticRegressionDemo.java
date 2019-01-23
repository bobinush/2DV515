package A4;

import java.io.IOException;
import java.util.Random;
import weka.classifiers.Classifier;
import weka.classifiers.Evaluation;
import weka.classifiers.functions.Logistic;
import weka.classifiers.functions.MultilayerPerceptron;
import weka.core.Instances;
import weka.core.converters.ConverterUtils.DataSource;

// Source: https://www.codingame.com/playgrounds/4821/machine-learning-with-java---part-2-logistic-regression
public class LogisticRegressionDemo {
	public Instances getDataSet(String fileName) throws IOException {
		Instances data = null;
		try {
			DataSource source = new DataSource(fileName);
			data = source.getDataSet();
			if (data.classIndex() == -1)
				   data.setClassIndex(data.numAttributes() - 1);
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return data;
	}
	
	public void startLogistic(Instances data) throws Exception {
		Classifier classifier = new Logistic();
		classifier.buildClassifier(data);
		
		Evaluation eval = new Evaluation(data);
		// Crossvalidate divide into bucket of size 10
		eval.crossValidateModel(classifier, data, 10, new Random(1));
		
		/** Print the algorithm summary */
		printInfo("Logistic", classifier, eval);
	}
	
	public void startMP(Instances data) throws Exception {
		Classifier classifier = new MultilayerPerceptron();
		classifier.buildClassifier(data);
		((MultilayerPerceptron) classifier).setHiddenLayers("72");

		Evaluation eval = new Evaluation(data);
		//eval.evaluateModel(classifier, testingDataSet);
		// Crossvalidate divide into bucket of size 10
		eval.crossValidateModel(classifier, data, 10, new Random(1));
		
		/** Print the algorithm summary */
		printInfo("MultilayerPerceptron", classifier, eval);
	}
	
	public void printInfo(String type, Classifier classifier, Evaluation eval) {
		System.out.println("** "+type+" **");
		System.out.println(eval.toSummaryString());
		System.out.print(" the expression for the input data as per alogorithm is ");
		System.out.println(classifier);
	}
}