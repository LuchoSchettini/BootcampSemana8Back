using Store.ApplicationCore.Helpers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.UnitTests.Utils
{
    public class DateUtilTests
    {

        [Fact]
        public void GetCurrentDate_ReturnsCorrectDate()
        {
            var CurrentDate = DateUtil.GetCurrentDate();

            Assert.True(CurrentDate.Year >= 2021);

        }

    }
}
