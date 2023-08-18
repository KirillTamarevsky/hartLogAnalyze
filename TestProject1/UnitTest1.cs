
using hartLogAnalyze.vm;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCreate()
        {
            var testHartLogString = getHartLogString();
            var analyzehartlogVM = new AnalyzeHARTLogVM(null, testHartLogString);
            Assert.IsNull(analyzehartlogVM);

        }
        string getHartLogString()
        {
            return "2023-08-09 15:34:07: DRIVER 1.4.0\r\n2023-08-09 15:34:07: DRVMAN SERVER 1.4.7\r\n2023-08-09 15:34:08: ASYNC CMD = 0 SEQID = 1, InvokeId: f4eeb67c-3fdb-4594-a353-da67e11a43be\r\n2023-08-09 15:34:08: HART Data Receive SEQID = 1\r\n2023-08-09 15:34:08: 40 Configuration changed\r\n2023-08-09 15:34:08: CMD  : <000> ADDRESS: <00> MASTER: <00>\r\n2023-08-09 15:34:08:         DATALENGTH: 0\r\n2023-08-09 15:34:08:         DATA: <>\r\n2023-08-09 15:34:08:         WRITE: FF FF FF FF FF 02 00 00 00 02 \r\n2023-08-09 15:34:08:         READ : FF FF FF FF FF FF 06 00 00 0E 00 40 FE 37 0B 05 \r\n2023-08-09 15:34:08:                05 03 0B 08 00 A8 AB 50 D9 \r\n2023-08-09 15:34:08:         STATE: <00 40>\r\n2023-08-09 15:34:08:         DATALENGTH: 12\r\n2023-08-09 15:34:08:         DATA: <FE 37 0B 05 05 03 0B 08 00 A8 AB 50>\r\n2023-08-09 15:34:08: ASYNC CMD = 13 SEQID = 2\r\n2023-08-09 15:34:09: HART Data Receive SEQID = 2\r\n2023-08-09 15:34:09: 40 Configuration changed\r\n2023-08-09 15:34:09: CMD  : <013> ADDRESS: <37 0B A8 AB 50> MASTER: <00>\r\n2023-08-09 15:34:09:         DATALENGTH: 0\r\n2023-08-09 15:34:09:         DATA: <>\r\n2023-08-09 15:34:09:         WRITE: FF FF FF FF FF 82 37 0B A8 AB 50 0D 00 E0 \r\n2023-08-09 15:34:09:         READ : FF FF FF FF FF FF 86 37 0B A8 AB 50 0D 17 00 40 \r\n2023-08-09 15:34:09:                19 4B 4A 40 2C 31 82 08 20 82 08 20 82 08 20 82 \r\n2023-08-09 15:34:09:                08 20 01 01 01 F7 \r\n2023-08-09 15:34:09:         STATE: <00 40>\r\n2023-08-09 15:34:09:         DATALENGTH: 21\r\n2023-08-09 15:34:09:         DATA: <19 4B 4A 40 2C 31 82 08 20 82 08 20 82 08 20 82 08 20 01 01 01>";
        }
    }
}