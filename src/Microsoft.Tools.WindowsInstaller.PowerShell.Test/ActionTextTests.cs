using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller
{
    [TestClass]
    public class ActionTextTests
    {
        [TestMethod]
        public void ActionText_GetActionText()
        {
            Assert.IsFalse(string.IsNullOrEmpty(ActionText.GetActionText("InstallFiles")));
        }

        [TestMethod]
        public void ActionText_GetActionText_Absent()
        {
            Assert.IsTrue(string.IsNullOrEmpty(ActionText.GetActionText("ShouldNotExist")));
        }

        [TestMethod]
        public void ActionText_GetActionData()
        {
            Assert.IsFalse(string.IsNullOrEmpty(ActionText.GetActionData("InstallFiles")));
        }

        [TestMethod]
        public void ActionText_GetActionData_Absent()
        {
            Assert.IsTrue(string.IsNullOrEmpty(ActionText.GetActionData("ShouldNotExist")));
        }
    }
}
