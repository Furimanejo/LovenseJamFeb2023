using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class ToyManager : MonoBehaviour
{
    static ToyManager instance = null;
    [SerializeField] TMP_InputField domain;
    [SerializeField] TMP_InputField portField;
    [SerializeField] Toggle enableVibrations;
    [SerializeField] Slider maxVibrationIntensity;
    public static string lovenseStatus = "";
    public static bool toysEnabled
    { 
        get
        {
            return instance.enableVibrations.isOn;
        } 
    }
    public static float vibrationScore = 0;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            StartCoroutine(PostCoroutine());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        vibrationScore -= 5 * Time.deltaTime;
        if (vibrationScore < 0)
            vibrationScore = 0;
    }

    public async void SendTest()
    {
        print("Send Test");
        vibrationScore = 25;
        await System.Threading.Tasks.Task.Delay(500);
        vibrationScore = 50;
        await System.Threading.Tasks.Task.Delay(500);
        vibrationScore = 75;
        await System.Threading.Tasks.Task.Delay(500);
        vibrationScore = 100;
        await System.Threading.Tasks.Task.Delay(500);
        vibrationScore = 0;
    }

    IEnumerator PostCoroutine()
    {
        UnityWebRequest request;
        var port = 20010;
        while (true)
        {
            if (enableVibrations.isOn)
            {
                var sendValue = (int)(maxVibrationIntensity.value * Mathf.Clamp(vibrationScore, 0f, 100f) / 100f);
                var command = new LovenseCommand()
                {
                    command = "Function",
                    action = $"Vibrate:{sendValue}",
                    timeSec = 2,
                    apiVer = 1
                };
                string postData = JsonUtility.ToJson(command);
                // Using the POST method will corrupt the json data, encoding manually instead
                var URL = $"http://{domain.text}:{port}/command";
                try
                {
                    request = new UnityWebRequest(URL, "POST");
                    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(postData);
                    request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");
                    request.certificateHandler = new BypassCertificateHandler();
                    request.timeout = 1;
                }
                catch
                {
                    request = null;
                }

                var timer = Time.time;
                if(request != null)
                {
                    yield return request.SendWebRequest();
                    print($"sent {sendValue} to {URL} => {request.responseCode} => {request.downloadHandler.text}");
                    if(request.responseCode != 200)
                    {
                        lovenseStatus = $"Connection error";
                    }
                    else
                    {
                        if (request.downloadHandler.text.Contains("\"code\":400"))
                        {
                            lovenseStatus = $"Finding port...";
                            port++;
                            if (port > 20020)
                                port = 20010;
                        }
                        else if(request.downloadHandler.text.Contains("\"code\":402"))
                        {
                            lovenseStatus = $"No toy connected...";
                        }
                        else
                        {
                            var ping = (int)(1000 * (Time.time - timer));
                            lovenseStatus = $"ping: {ping} ms";
                        }
                    }
                    request.disposeCertificateHandlerOnDispose = true;
                    request.disposeDownloadHandlerOnDispose = true;
                    request.disposeUploadHandlerOnDispose = true;
                    request.Dispose();
                }
            }
            else
            {
                lovenseStatus = "Not enabled";
            }
            yield return new WaitForEndOfFrame();
        }
    }

    struct LovenseCommand
    {
        public string command;
        public string action;
        public double timeSec;
        public int apiVer;
    }

    class BypassCertificateHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}
