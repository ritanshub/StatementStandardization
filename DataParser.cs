namespace Onebanq{

    public interface IDataParser{
        TransactionDetail Parsedata(string data,string[] header,TransactionType transtype,string name);
    }

    public abstract class DataParser :IDataParser{
        public abstract TransactionDetail Parsedata(string data,string[] header,TransactionType transtype,string name);

        public virtual string getDate(string date){
            string[] dateerr= date.Split("-");
            DateTime newdate= new DateTime(Convert.ToInt32(dateerr[2]),Convert.ToInt32(dateerr[1]),Convert.ToInt32(dateerr[0]));
            return newdate.ToString("dd/MM/yyyy");
        }
        public virtual List<string> cleanData(string[] tempdata){
            var res=new List<string>();
            foreach (var item in tempdata)
            {
                if(!string.IsNullOrWhiteSpace(item))
                    res.Add(item);
            }
            return res;
        }
    }
}