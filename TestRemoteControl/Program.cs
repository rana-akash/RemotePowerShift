namespace TestRemoteControl;

internal class Program
{
    private static async Task Main(string[] args)
    {
    //loop start
        var client = new HttpClient();
        client.BaseAddress = new Uri("http://aranaapi1-evgreegzhjeqh8fw.canadacentral-01.azurewebsites.net");

        var getResult = await client.GetAsync("/Command");
        var res = await getResult.Content.ReadAsStringAsync();
        if (res == "true")
        {
            HttpContent content = new StringContent("false");
            var postResult = await client.PostAsync("/Command", content);
            res = await postResult.Content.ReadAsStringAsync();
            var cpuStatus = "OFF"; //Get this from input pin
            if (res == "success" && cpuStatus == "OFF")
            {
                // Turn on CPU    
            }
        }
        //delay(2 mins)
    //loop end
    }
}