/*
 * Сделано в SharpDevelop.
 * Пользователь: 055makarov
 * Дата: 16.10.2014
 * Время: 9:46
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.ServiceModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using RemoteLaunchAppHost;
using System.Threading;
namespace RemoteLaunchAppHost
{
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
	public class RemoteLaunchAppService : IRemoteLaunchAppService,IDisposable
	{
		Action<string> receiveData;
		Process _p;
		TimeSpan _processLifeTime;
        Timer _processTimer;
        //stream writer для процесса с правильной кодировкой
        StreamWriter _sw;
        string _consoleEncoding = "cp866";
	   public void LaunchProg(string command,string args,TimeSpan timeToExit)
	   {
	   		try
	   		{
	   		
	   			_processLifeTime=timeToExit;
	   			_processTimer=new Timer(CloseProcess,null,(int)timeToExit.TotalMilliseconds,Timeout.Infinite);
	   			ProcessStartInfo psi = new ProcessStartInfo();
	   			psi.FileName=command;
				psi.UseShellExecute=false;
				psi.StandardOutputEncoding=Encoding.GetEncoding(_consoleEncoding);
                psi.StandardErrorEncoding = Encoding.GetEncoding(_consoleEncoding);
				psi.RedirectStandardOutput=true;	
				psi.RedirectStandardInput =true;
				psi.RedirectStandardError = true;
            
	   			psi.Arguments=args;
              	
	   			_p = new Process();
	   			_p.EnableRaisingEvents=true;
	   			_p.StartInfo=psi;
			
	   			_p.OutputDataReceived+= new DataReceivedEventHandler(p_OutputDataReceived);
	   			_p.ErrorDataReceived+= new DataReceivedEventHandler(p_OutputDataReceived);
	   			_p.Exited+= new EventHandler(p_Exited);
       
	   			_p.Start();
                _sw = new StreamWriter(_p.StandardInput.BaseStream, Encoding.GetEncoding(_consoleEncoding)) { AutoFlush=true};
	   			_p.BeginOutputReadLine();
	   			_p.BeginErrorReadLine();
	   		}
	   		catch(Exception ex)
	   		{
	   			Debugger.Log(0,"exception",ex.Message);
                throw new FaultException(ex.Message);

	   		}
	   }

	   public void CloseProg()
	   {
	   		CloseProcess(null);
	   }
	   
	   public void SendMessage(string command)
	   {
	   		if(_p!=null && _sw!=null)
	   		{
                _sw.WriteLine(command);
	   		}
	   }
	   
	   void CloseProcess(object state)
	   {
           if (_sw != null)
           {
               _sw.Close();
           }
           
            if(_p!=null)
	   		{
	   			_p.OutputDataReceived -= new DataReceivedEventHandler(p_OutputDataReceived); 
	   			_p.ErrorDataReceived-= new DataReceivedEventHandler(p_OutputDataReceived);
                _p.Close();
	   		}

	   }
	   
	   void p_Exited(object sender, EventArgs e)
	   {
	   		Debugger.Log(0,"eventAction","process exited");
			_p.Close();
	   }

	   void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
	   {
	   		try{
	   				Debugger.Log(0,"eventAction",string.Format("data received: {0}",e.Data));
	   				if(receiveData!=null)
                       receiveData(e.Data);	
			}
			catch(Exception ex)
			{
				Debugger.Log(0,"exception",ex.Message);
                //throw new FaultException(ex.Message);
			}
	   }
	   
	   public void SubscribeRecieved()
        {

           receiveData = OperationContext.Current.GetCallbackChannel<IHostCallback>().RecievedData;
    	}
	   
	   public void Dispose()
	   {
	   	if(_p!=null){
	   		_p.Dispose();
	   	}
	   	if(_processTimer!=null)
	   		_processTimer.Dispose();

        if (_sw != null)
        {
            
            _sw.Dispose();
        }
	   }

       
	  
	} 
}
