using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LilyAPNNotification.Model;

namespace LilyAPNNotification.Cache
{
    public class NotificationServiceCache
    {
        private static Queue<Task> ExistQueue = null;
        private static Queue<Task> ExcuteList = null;
        private static int ExcNumber = 50;        

        public static void ExcuteNotifyPush(Task task) 
        {
            AddNotifyService(task);
            ExcuteTask();
        }        

        private static void AddNotifyService(Task task){
            var myQueue = GetExistQueue();
            myQueue.Enqueue(task);
        }

        private static void ExcuteTask(){
            var myExcuteQueue = GetExcuteQueue();
            var myQueue = GetExistQueue();

            // remove the ended task
            int len = ExcuteList.Count;
            int cntPlot = 0;
            while (ExcuteList.Count > 0) {
                if (ExcuteList.Peek().IsEnded) {
                    ExcuteList.Dequeue();
                    cntPlot++;
                }
            }
            if (cntPlot == 0 && ExcuteList.Count == 0) {
                cntPlot = ExcNumber;
            }

            // once time excute maybe 50(can modify) tasks
            for (int i = 0; i < cntPlot; i++)
            {
                if (myQueue.Count > 0)
                {
                    var task = myQueue.Dequeue();
                    ExcuteList.Enqueue(task);
                    task.Excute();
                }
                else break;
            }
        }

        public static Queue<Task> GetExistQueue(){
            if(ExistQueue == null){                
                ExistQueue = new Queue<Task>();                
            }
            return ExistQueue;
        }

        public static Queue<Task> GetExcuteQueue()
        {
            if (ExcuteList == null)
            {
                ExcuteList = new Queue<Task>();
            }
            return ExcuteList;
        }    
    }
}
