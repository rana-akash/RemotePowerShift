#include <SPI.h>
#include <HttpClient.h>
#include <Ethernet.h>
#include <EthernetClient.h>
#include <string.h>

byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };

// Number of milliseconds to wait without receiving any data before we give up
const int kNetworkTimeout = 30*1000;
// Number of milliseconds to wait if no data is available before trying again
const int kNetworkDelay = 1000;
void printCharArr(char *apiResult);
void ClearArray(char *arr);
int GetCpuState();
void TurnCpuOn();

void setup()
{
  Serial.begin(9600);
}

void loop()
{
  int CpuState = 0;
  unsigned long timeoutStart;
  char c;
  char apiResult[10];
  int apiResultIndex = 0;
  int err;
  int getResponse;
  int relevantChar = 0;
  int skipped;
  EthernetClient ethClient;
  HttpClient http(ethClient);
  char kHostname[] = "aranaapi1-evgreegzhjeqh8fw.canadacentral-01.azurewebsites.net";
  char kGetPath[] = "/Command/GetCommand";
  char kPostPath[] = "/Command/PostCommand?input=false";
  
  //Get CpuState, if ON, goto moveOn label
  CpuState = GetCpuState();
  if(CpuState == 1){
    Serial.println("already ON..");
    goto moveOn;
  }

  Ethernet.init(17);
  while (Ethernet.begin(mac) != 1)
  {
    Serial.println("Error getting IP address via DHCP, trying again...");
    goto stopAndDoAgain;
  }

  err = http.get(kHostname, kGetPath);

  if (err != 0) //no error
  {
    Serial.print("Connect failed: ");
    Serial.println(err);
    goto stopAndDoAgain;
  }

  getResponse = http.responseStatusCode();

  if(getResponse != 200){
    Serial.print("Err : response status is : ");
    Serial.println(getResponse);
    goto stopAndDoAgain;
  }

  skipped = http.skipResponseHeaders();
  if(skipped < 0){
    Serial.println("response headers couldn't be skipped.");
    Serial.println(skipped);
    goto stopAndDoAgain;
  }

  timeoutStart = millis();

  while ( (http.connected() || http.available()) && ((millis() - timeoutStart) < kNetworkTimeout) )
  {
      if (http.available())
      {
          c = http.read();

          if(c == 93){ // ] character
            relevantChar = 0;
          }

          if(relevantChar == 1){
            // Serial.print(c);
            apiResult[apiResultIndex] = c;
            apiResultIndex++;
          }

          if(c == 91){ // [ character
            relevantChar = 1;
          }

          timeoutStart = millis();
      }
      else
      {
        delay(kNetworkDelay);
      }
  }
  relevantChar = 0; //incase its not set by response;
  apiResult[apiResultIndex] = '\0';
  http.stop();

  if (strcmp(apiResult, "True") == 0)
  {
    apiResultIndex = 0;
    ClearArray(apiResult);

    Serial.println("GetCommand Result is true");

    err = http.get(kHostname, kPostPath);

    if (err != 0) //no error
    {
      Serial.print("Connect failed: ");
      Serial.println(err);
      goto stopAndDoAgain;
    }

    getResponse = http.responseStatusCode();

    if(getResponse != 200){
      Serial.print("Err : response status is : ");
      Serial.println(getResponse);
      goto stopAndDoAgain;
    }

    skipped = http.skipResponseHeaders();
    if(skipped < 0){
      Serial.println("response headers couldn't be skipped.");
      Serial.println(skipped);
      goto stopAndDoAgain;
    }

    timeoutStart = millis();
    
    while ( (http.connected() || http.available()) && ((millis() - timeoutStart) < kNetworkTimeout) )
    {
        if (http.available())
        {
            c = http.read();

            if(c == 93){ // ] character
              relevantChar = 0;
            }

            if(relevantChar == 1){
              // Serial.print(c);
              apiResult[apiResultIndex] = c;
              apiResultIndex++;
            }

            if(c == 91){ // [ character
              relevantChar = 1;
            }

            timeoutStart = millis();
        }
        else
        {
          delay(kNetworkDelay);
        }
    }
    apiResult[apiResultIndex] = '\0';
    if (strcmp(apiResult, "Success") == 0)
    {
      // Get CPU state in CPUState var
      Serial.println("PostCommand Result is Success");
      CpuState = GetCpuState();
      if(CpuState == 1){
        Serial.println("already ON..");
        goto moveOn;
      }
      else if(CpuState == 0){
        //Turn ON CPU
        Serial.println("Turning Cpu ON....");
        TurnCpuOn();
      }
    }
  }

stopAndDoAgain:
  http.stop();
moveOn:
  delay(30 * 1000);
}

void TurnCpuOn(){

}

int GetCpuState(){
  return 0;
}

void printCharArr(char *apiResult){
  int i = 0;
  while (1) {
    if(apiResult[i] == '\0'){
      return;
    }
    Serial.print(apiResult[i]);
    i++;
  }
}

void ClearArray(char *arr){
  int len = 10;
  for (int i = 0; i<=len; i++) {
    arr[i] = '\0';
  }
}





