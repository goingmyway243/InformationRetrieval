// See https://aka.ms/new-console-template for more information

using InformationRetrieval;

var docs = File.ReadAllText("../../../Data/doc-text").Split('/').ToList();
var queries = File.ReadAllText("../../../Data/query-text").Split('/').ToList();
var stopWords = File.ReadAllText("../../../Data/stop-words.txt").Split("\r\n").ToList();

var query = "switching circuits using junction";

var optionStr =
    "1. Boolean Model Retrieval\n"
    + "2. Inverted Index\n"
    + "3. Vector Space Model\n"
    + "4. VB Code\n"
    + "\nChoose one: ";

Console.Write(optionStr);
var choose = Console.ReadLine();

switch (int.Parse(choose))
{
    case 1:
        {
            var booleanModelRetrieval = new BooleanRetrievalModel(docs, stopWords);
            booleanModelRetrieval.GetResultAtQueryNumber(query);
            break;
        }
    case 2:
        {
            var invertedIndex = new InvertedIndex(docs, stopWords);
            invertedIndex.GetResultAtQueryNumber(query, 2);
            break;
        }
    case 3:
        {
            var vectorSpaceModel = new VectorSpaceModel(docs, stopWords);
            vectorSpaceModel.GetValueFromQuery(query);
            break;
        }
    case 4:
        {
            var test = new List<int>() { 824, 5, 214577 };
            var docIDCompression = new DocIDCompression();

            var encoded = docIDCompression.EncodeVB(test);

            var resultStr = docIDCompression.ToBinaryNumberString(encoded);
            Console.WriteLine("Encode VB:");
            foreach (var res in resultStr)
            {
                Console.WriteLine(res);
            }

            Console.WriteLine();

            var decoded = docIDCompression.DecodeVB(encoded);
            Console.WriteLine("Decode VB:");
            foreach (var res in decoded)
            {
                Console.WriteLine(res);
            }

            break;
        }
    case 5:
        {
            var test = new List<int>() { 9, 13, 24, 511, 1025 };
            var docIDCompression = new DocIDCompression();
            var encoded = docIDCompression.EncodeGamma(test);

            Console.WriteLine("Encode Gamma:");
            foreach (var res in encoded)
            {
                Console.WriteLine(res);
            }

            Console.WriteLine();

            var decoded = docIDCompression.DecodeGamma(encoded);
            Console.WriteLine("Decode Gamma:");
            foreach (var res in decoded)
            {
                Console.WriteLine(res);
            }

            break;
        }
    default:
        break;
}







