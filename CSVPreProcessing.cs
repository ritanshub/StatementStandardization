using System.Diagnostics;
using System.Data;
using System.Text;
namespace Onebanq
{
    public class CSVPreProcessing{

        private IDataParser _dataPareser;
        public CSVPreProcessing(IDataParser dataParser){
            _dataPareser=dataParser;
        }

        public List<TransactionDetail> Process(string filename){
            var preProcessTransData= preProcess(filename);
            var transdetails= new List<TransactionDetail>();
            TransactionType transactionType=TransactionType.Domestic;
            string name="";
            string[] header =null;
            foreach (var item in preProcessTransData)
            {
                if(item.MetaData=="TransactionType") transactionType=item.Data.Contains("Domestic")?TransactionType.Domestic:TransactionType.International;
                if(item.MetaData=="HolderName") name= item.Data;
                if(item.MetaData== "Header") header= item.Data.Split(",");
                if(item.MetaData=="Data"){
                    var transdetail=_dataPareser.Parsedata(item.Data,header,transactionType,name);
                    transdetails.Add(transdetail);
                }
            }
            WriteCSV(transdetails,filename);
            return transdetails;
        }





        private class PreProcessTransData{
            public string MetaData{get;set;}
            public string Data{get;set;}
            
        }

        private List<PreProcessTransData> preProcess(string fileName){
            string line;
            List<PreProcessTransData> preProcessTransData= new List<PreProcessTransData>();
            string file= @"D:\Onebanq\Doc\"+fileName+".csv";
            using(var stream= File.OpenRead(file))
            using(var sr =new StreamReader(stream)){
                while((line=sr.ReadLine())!=null){
                    var temp= getRowData(line);
                    if(temp!=null) preProcessTransData.Add(temp);

                }
            }
            return preProcessTransData;
        }

        private PreProcessTransData getRowData(string line)
        {
            string[] row= line.Split(',');
            int count=0;
            foreach (var item in row)
            {
                if(!string.IsNullOrWhiteSpace(item))
                    count++;
            }
            PreProcessTransData res=null;
            bool isField= (count==1);
            bool isData=(count>1) && line.Any(char.IsDigit);
            bool isHeader= (count>1) && !isData;
            if(isField){
                if(line.Contains("Transaction")) res= new PreProcessTransData{MetaData= "TransactionType",Data=line.Replace(",","")};
                else res=new PreProcessTransData{MetaData="HolderName", Data=line.Replace(",","")};

            }
            else if(isHeader){
                res=new PreProcessTransData{ MetaData="Header", Data=line};

            }
            else if(isData){
                res=new PreProcessTransData{ MetaData="Data", Data=line};
                
            }
            else
              return null;

            return res;
        }
        private void WriteCSV(List<TransactionDetail> trans, string filename){
            var csv= new StringBuilder();
            var header= string.Format("Date,Transaction Description,Debit,Credit,Currency,CardName,Transaction,Location");
            csv.AppendLine(header);
            foreach (var item in trans)
            {
                var newLine= string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",item.date,item.Description,item.Debit,item.Credit,item.Currency,item.HolderName,item.TransactionType,item.Location);
                csv.AppendLine(newLine);
            }
            filename=filename.Replace("Input","Output");
            string path= @"D:\Onebanq\Doc\"+filename+".csv";
            File.WriteAllText(path,csv.ToString());
        }
    }
}