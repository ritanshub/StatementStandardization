namespace Onebanq{
    public class IDFCParser :DataParser{
        public override  TransactionDetail Parsedata(string data,string[] header,TransactionType transtype,string name){
            var arrdata= data.Split(",");
            var transdetail= new TransactionDetail();
            transdetail.HolderName= name;
            transdetail.TransactionType=transtype;
            if(header != null){
                for(int i=0;i<header.Length;i++){
                    if(Convert.ToString(header[i]).Trim()=="Date")
                        transdetail.date= getDate(arrdata[i]);
                  
                    else if(Convert.ToString(header[i]).Trim()=="Amount"){
                        if(arrdata[i].ToLower().Contains("cr")){
                            string temp= arrdata[i].ToLower().Replace("cr","");
                            Double credit= Convert.ToDouble(temp);
                            transdetail.Credit=credit;
                        }
                        else
                            transdetail.Debit=Convert.ToDouble(arrdata[i]);
                    }
                    else if(Convert.ToString(header[i]).Trim()=="Transaction Details"){
                        string[] tempdata=Convert.ToString(arrdata[i]).Split(" ");
                        var desdata= cleanData(tempdata);
                        string des="";

                        if(transdetail.TransactionType==TransactionType.Domestic){
                            for(int j=0;j<desdata.Count;j++){
                                if(j==desdata.Count-1) transdetail.Location= desdata[j];
                                else des=des+desdata[j]+" ";
                            }
                            transdetail.Description= des;
                            transdetail.Currency="INR";

                        }
                        else{
                            for(int j=0;j<desdata.Count;j++){
                                if(j==desdata.Count-1) transdetail.Currency= desdata[j];
                                else if(j==desdata.Count-2) transdetail.Location= desdata[j];
                                else des=des+desdata[j]+" ";
                            }
                            transdetail.Description= des;
                            //transdetail.Currency="INR";

                        }
                    }
                }
            }
            return transdetail;
        }

        public override string getDate(string date){
            // string[] dateerr= date.Split("-");
            // DateTime newdate= new DateTime(Convert.ToInt32(dateerr[2]),Convert.ToInt32(dateerr[1]),Convert.ToInt32(dateerr[0]));
            return (Convert.ToDateTime(date)).ToString("dd/MM/yyyy");
        }

    }

}