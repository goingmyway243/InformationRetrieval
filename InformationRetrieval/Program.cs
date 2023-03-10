// See https://aka.ms/new-console-template for more information

using InformationRetrieval;

var docs = File.ReadAllText("../../../Data/doc-text").Split('/').ToList();
var queries = File.ReadAllText("../../../Data/query-text").Split('/').ToList();
var stopWords = File.ReadAllText("../../../Data/stop-words.txt").Split("\r\n").ToList();

/*var booleanModelRetrieval = new BooleanRetrievalModel(docs, queries, stopWords);
booleanModelRetrieval.GetResultAtQueryNumber(1);*/

/*var invertedIndex = new InvertedIndex(docs, queries, stopWords);
invertedIndex.GetResultAtQueryNumber(1, 2);*/

var vectorSpaceModel = new VectorSpaceModel(docs, stopWords);
vectorSpaceModel.GetValueFromQuery("switching circuits using junction");
