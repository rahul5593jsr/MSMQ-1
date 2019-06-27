using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MsMq
{
    class Program
    {
        static void Main(string[] args)
        {
            const string queueName = @".\private$\FRECODEX.NGRESULTS.IN";
            SendMessageToQueue(queueName);
            //ReceiveMessageFromQueue(queueName);
            Console.ReadKey();
        }
        private static void SendMessageToQueue(string queueName)
        {
            MessageQueue msMq = null;           
            if (!MessageQueue.Exists(queueName))
            {
                msMq = MessageQueue.Create(queueName,true);
            }
            else
            {
                msMq = new MessageQueue(queueName);
            }
            try
            {
                using (var tr = new MessageQueueTransaction())
                {                  
                    Person p = new Person()
                    {
                        FirstName = "Rahul",
                        LastName = "Kumar"
                    };
                    tr.Begin();
                    msMq.Send(p);
                    tr.Commit();
                }
            }
            catch (MessageQueueException ee)
            {
                Console.Write(ee.ToString());
            }
            finally
            {
                msMq.Close();
            }
            Console.WriteLine("Message sent ......");
        }

        private static void ReceiveMessageFromQueue(string queueName)
        {
            MessageQueue msMq = msMq = new MessageQueue(queueName);
            try
            {
                using (var tr = new MessageQueueTransaction())
                {
                    msMq.Formatter = new XmlMessageFormatter(new Type[] { typeof(Person) });
                    tr.Begin();
                    var message = (Person)msMq.Receive().Body;
                    tr.Commit();
                    Console.WriteLine("FirstName: " + message.FirstName + ", LastName: " + message.LastName);
                }
            }
            catch (MessageQueueException ex)
            {
                Console.Write(ex.ToString());

            }
            finally
            {
                msMq.Close();
            }
            Console.WriteLine("Message received ......");
        }
    }
}
