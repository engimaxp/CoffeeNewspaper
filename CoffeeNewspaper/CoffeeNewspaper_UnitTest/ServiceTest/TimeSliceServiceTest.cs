using System;
using System.Collections.Generic;
using System.Linq;
using CN_BLL;
using CN_Model;
using CN_Repository;
using CoffeeNewspaper_UnitTest.DomainTest;
using NUnit.Framework;
using NSubstitute;
namespace CoffeeNewspaper_UnitTest.ServiceTest
{
    [TestFixture]
    public class TimeSliceServiceTest
    {
        [Test]
        public void GetTaskTimeSlices_WithTaskStartTime_SingleDay()
        {
            var targetRespository = Substitute.For<ITimeSliceProvider>();
            TimeSliceService targetService = new TimeSliceService(targetRespository);
            var testTask = DomainTestHelper.GetARandomTask(1);
            var currentTime = DateTime.Now;
            var testslice = new CNTimeSlice(currentTime.AddMinutes(-3), currentTime);
            var expected = new Dictionary<int, List<CNTimeSlice>> {{ testTask.TaskId, new List<CNTimeSlice>() {testslice}}};
            targetRespository.GetOriginalDataByDate(DateTime.Now.AddDays(-1).ToString(CNConstants.DIRECTORY_DATEFORMAT))
                .Returns(expected);

            testTask.StartTime = DateTime.Now.AddDays(-1);

            var result = targetService.GetTaskTimeSlices(testTask);
            targetRespository.Received()
                .GetOriginalDataByDate(DateTime.Now.AddDays(-1).ToString(CNConstants.DIRECTORY_DATEFORMAT));
            Assert.AreEqual(result, expected[testTask.TaskId]);
        }
        [Test]
        public void GetTaskTimeSlices_WithoutTaskStartTime()
        {
            var targetRespository = Substitute.For<ITimeSliceProvider>();
            TimeSliceService targetService = new TimeSliceService(targetRespository);

            var testTask = DomainTestHelper.GetARandomTask(1);
            var currentTime = DateTime.Now;
            var testslice = new CNTimeSlice(currentTime.AddMinutes(-3), currentTime);
            var expected = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice } } };
            targetRespository.GetOriginalDataByDate(Arg.Any<string>())
                .Returns(expected);

            testTask.StartTime = null;
            var result = targetService.GetTaskTimeSlices(testTask);

            Assert.AreEqual(result, new List<CNTimeSlice>());
            targetRespository.DidNotReceiveWithAnyArgs()
                .GetOriginalDataByDate(Arg.Any<string>());
            
        }
        [Test]
        public void GetTaskTimeSlices_WithTaskStartTime_NoEndTime_MutipleDay()
        {
            var targetRespository = Substitute.For<ITimeSliceProvider>();
            TimeSliceService targetService = new TimeSliceService(targetRespository);
            var testTask = DomainTestHelper.GetARandomTask(1);
            var currentTime = DateTime.Now;
            var testslice1 = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-3), currentTime.AddDays(-1));
            var testslice2 = new CNTimeSlice(currentTime.AddMinutes(-3), currentTime);
            var testslice3 = new CNTimeSlice(currentTime.AddDays(1).AddMinutes(-3), currentTime.AddDays(1));
            var expected = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice1, testslice2 } } };
            var expected1 = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice1 } } };
            var expected2 = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() {  testslice2 } } };
            var expected3 = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() {  testslice3 } } };
            targetRespository.GetOriginalDataByDate(testslice1.StartDate)
                .Returns(expected1);
            targetRespository.GetOriginalDataByDate(testslice2.StartDate)
                .Returns(expected2);
            targetRespository.GetOriginalDataByDate(testslice3.StartDate)
                .Returns(expected3);
            testTask.StartTime = DateTime.Now.AddDays(-1);

            var result = targetService.GetTaskTimeSlices(testTask);
            targetRespository.Received()
                .GetOriginalDataByDate(DateTime.Now.AddDays(-1).ToString(CNConstants.DIRECTORY_DATEFORMAT));
            Assert.AreEqual(0, result.Except(expected[testTask.TaskId]).Count());
        }

        [Test]
        public void GetTaskTimeSlices_WithTaskStartTime_EndTime_MutipleDay()
        {
            var targetRespository = Substitute.For<ITimeSliceProvider>();
            TimeSliceService targetService = new TimeSliceService(targetRespository);
            var testTask = DomainTestHelper.GetARandomTask(1);
            var currentTime = DateTime.Now;
            var testslice1 = new CNTimeSlice(currentTime.AddDays(-2).AddMinutes(-3), currentTime.AddDays(-2));
            var testslice2 = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-3), currentTime.AddDays(-1));
            var testslice3 = new CNTimeSlice(currentTime.AddMinutes(-3), currentTime);
            var expected = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice1, testslice2 } } };
            var expected1 = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice1 } } };
            var expected2 = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice2 } } };
            var expected3 = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice3 } } };
            targetRespository.GetOriginalDataByDate(testslice1.StartDate)
                .Returns(expected1);
            targetRespository.GetOriginalDataByDate(testslice2.StartDate)
                .Returns(expected2);
            targetRespository.GetOriginalDataByDate(testslice3.StartDate)
                .Returns(expected3);
            testTask.StartTime = DateTime.Now.AddDays(-2);
            testTask.EndTime = DateTime.Now.AddDays(-1);

            var result = targetService.GetTaskTimeSlices(testTask);
            targetRespository.Received()
                .GetOriginalDataByDate(DateTime.Now.AddDays(-1).ToString(CNConstants.DIRECTORY_DATEFORMAT));
            Assert.AreEqual(0, result.Except(expected[testTask.TaskId]).Count());
        }
        [Test]
        public void AddATimeSlice_InterceptNotExists()
        {
            //Arrange
            var targetRespository = Substitute.For<ITimeSliceProvider>();
            TimeSliceService targetService = new TimeSliceService(targetRespository);
            var testTask = DomainTestHelper.GetARandomTask(1);
            var currentTime = DateTime.Now;
            var testslice = new CNTimeSlice(currentTime.AddMinutes(-3), currentTime);
            var expected = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice } } };
            targetRespository.GetOriginalDataByDate(testslice.StartDate)
                .Returns(expected);
            //Act
            var addedData = new CNTimeSlice(currentTime.AddMinutes(1), currentTime.AddMinutes(3));
            targetService.AddATimeSlice(testTask, addedData);
            //Assert
            targetRespository.Received()
                .OverWriteToDataSourceByDate(testslice.StartDate, Arg.Is< Dictionary<int, List<CNTimeSlice>>>(x=>x.ContainsKey(testTask.TaskId) && !new List<CNTimeSlice>() { testslice, addedData }.Except(x[testTask.TaskId]).Any() ));
        }
        [Test]
        public void AddATimeSlice_InterceptExists_throwsArgException()
        {
            //Arrange
            var targetRespository = Substitute.For<ITimeSliceProvider>();
            TimeSliceService targetService = new TimeSliceService(targetRespository);
            var testTask = DomainTestHelper.GetARandomTask(1);
            var currentTime = DateTime.Now;
            var testslice = new CNTimeSlice(currentTime.AddMinutes(-3), currentTime);
            testTask.StartTime = testslice.StartDateTime;
            testTask.EndTime = testslice.EndDateTime;
            var expected = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice } } };
            targetRespository.GetOriginalDataByDate(testslice.StartDate)
                .Returns(expected);
            //Act
            var addedData = new CNTimeSlice(currentTime.AddMinutes(-1), currentTime.AddMinutes(3));
            Assert.Throws<ArgumentException>(() => {
                targetService.AddATimeSlice(testTask, addedData);
            });
        }

        [Test]
        public void AddATimeSlice_MutipleDay_InterceptNotExists()
        {
            //Arrange
            var targetRespository = Substitute.For<ITimeSliceProvider>();
            TimeSliceService targetService = new TimeSliceService(targetRespository);
            var testTask = DomainTestHelper.GetARandomTask(1);
            var currentTime = DateTime.Now;
            var testslice1 = new CNTimeSlice(currentTime.AddDays(-2).AddMinutes(-3), currentTime.AddDays(-1).AddMinutes(-2));
            var testslice2 = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-1), currentTime.AddDays(-1).AddMinutes(2));
            testTask.StartTime = testslice1.StartDateTime;
            testTask.EndTime = testslice2.EndDateTime;
            var expected = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice1, testslice2 } } };
            var expected1 = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice1 } } };
            var expected2 = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice2 } } };
            targetRespository.GetOriginalDataByDate(testslice1.StartDate)
                .Returns(expected1);
            targetRespository.GetOriginalDataByDate(testslice2.StartDate)
                .Returns(expected2);
            //Act
            var addedData = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-2), currentTime.AddDays(-1).AddMinutes(-1));
                targetService.AddATimeSlice(testTask, addedData);

            targetRespository.Received()
                .OverWriteToDataSourceByDate(addedData.StartDate, Arg.Is<Dictionary<int, List<CNTimeSlice>>>(x => x.ContainsKey(testTask.TaskId) && !new List<CNTimeSlice>() { testslice2, addedData }.Except(x[testTask.TaskId]).Any()));
        }
        [Test]
        public void AddATimeSlice_MutipleDay_InterceptExists_throwException()
        {
            //Arrange
            var targetRespository = Substitute.For<ITimeSliceProvider>();
            TimeSliceService targetService = new TimeSliceService(targetRespository);
            var testTask = DomainTestHelper.GetARandomTask(1);
            var currentTime = DateTime.Now;
            var testslice1 = new CNTimeSlice(currentTime.AddDays(-2).AddMinutes(-3), currentTime.AddDays(-1).AddMinutes(-2));
            var testslice2 = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-1), currentTime.AddDays(-1).AddMinutes(2));
            testTask.StartTime = testslice1.StartDateTime;
            testTask.EndTime = testslice2.EndDateTime;
            var expected = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice1, testslice2 } } };
            var expected1 = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice1 } } };
            var expected2 = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice2 } } };
            targetRespository.GetOriginalDataByDate(testslice1.StartDate)
                .Returns(expected1);
            targetRespository.GetOriginalDataByDate(testslice2.StartDate)
                .Returns(expected2);
            //Act
            var addedData = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-3), currentTime.AddDays(-1).AddMinutes(-1));
            
            Assert.Throws<ArgumentException>(() => {
                targetService.AddATimeSlice(testTask, addedData);
            });
        }
        [Test]
        public void AddATimeSlice_DontChangeStartEndTime()
        {
            //Arrange
            var rootDataRepository = Substitute.For<IRootDataProvider>();
            var targetRespository = Substitute.For<ITimeSliceProvider>();
            TimeSliceService targetService = new TimeSliceService(targetRespository, rootDataRepository);
            var testTask = DomainTestHelper.GetARandomTask(1);
            var currentTime = DateTime.Now;
            var testslice1 = new CNTimeSlice(currentTime.AddDays(-2).AddMinutes(-3), currentTime.AddDays(-1).AddMinutes(-2));
            var testslice2 = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-1), currentTime.AddDays(-1).AddMinutes(2));
            testTask.StartTime = testslice1.StartDateTime;
            testTask.EndTime = testslice2.EndDateTime;
            var expected = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice1, testslice2 } } };
            var expected1 = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice1 } } };
            var expected2 = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice2 } } };
            targetRespository.GetOriginalDataByDate(testslice1.StartDate)
                .Returns(expected1);
            targetRespository.GetOriginalDataByDate(testslice2.StartDate)
                .Returns(expected2);
            var expectedRoot = new CNRoot();
            expectedRoot.AddOrUpdateTask(testTask);
            rootDataRepository.GetRootData().Returns(expectedRoot);
            //Act
            var addedData = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-2), currentTime.AddDays(-1).AddMinutes(-1));
            targetService.AddATimeSlice(testTask, addedData);

            //Assert
            rootDataRepository.DidNotReceive().Persistence(Arg.Any<CNRoot>());
        }
        [Test]
        public void AddATimeSlice_ChangeTaskStartEndTime()
        {
            //Arrange
            var rootDataRepository = Substitute.For<IRootDataProvider>();
            var targetRespository = Substitute.For<ITimeSliceProvider>();
            TimeSliceService targetService = new TimeSliceService(targetRespository,rootDataRepository);
            var testTask = DomainTestHelper.GetARandomTask(1);
            var currentTime = DateTime.Now;
            var testslice1 = new CNTimeSlice(currentTime.AddDays(-2).AddMinutes(-3), currentTime.AddDays(-1).AddMinutes(-2));
            var testslice2 = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-1), currentTime.AddDays(-1).AddMinutes(2));
            testTask.StartTime = testslice1.StartDateTime;
            testTask.EndTime = testslice2.EndDateTime;
            var expected1 = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice1 } } };
            var expected2 = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice2 } } };
            targetRespository.GetOriginalDataByDate(testslice1.StartDate)
                .Returns(expected1);
            targetRespository.GetOriginalDataByDate(testslice2.StartDate)
                .Returns(expected2);
            var expectedRoot = new CNRoot();
            expectedRoot.AddOrUpdateTask(testTask);
            rootDataRepository.GetRootData().Returns(expectedRoot);
            //Act
            var addedData = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(2), currentTime.AddDays(-1).AddMinutes(4));
            targetService.AddATimeSlice(testTask, addedData);

            //Assert
            rootDataRepository.Received().Persistence(Arg.Is<CNRoot>(x => x.GetTaskById(testTask.TaskId).StartTime == testTask.StartTime &&
                       x.GetTaskById(testTask.TaskId).EndTime == addedData.EndDateTime));
        }

        [Test]
        public void DeleteATimeSlice_NotChangingTaskStartEndTime()
        {
            //Arrange
            var rootDataRepository = Substitute.For<IRootDataProvider>();
            var targetRespository = Substitute.For<ITimeSliceProvider>();
            TimeSliceService targetService = new TimeSliceService(targetRespository, rootDataRepository);
            var testTask = DomainTestHelper.GetARandomTask(1);
            var currentTime = DateTime.Now;
            var testslice1 = new CNTimeSlice(currentTime.AddDays(-2).AddMinutes(-3), currentTime.AddDays(-1).AddMinutes(-2));
            var testslice2 = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-2), currentTime.AddDays(-1).AddMinutes(-1));
            var testslice3 = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-1), currentTime.AddDays(-1).AddMinutes(2));
            testTask.StartTime = testslice1.StartDateTime;
            testTask.EndTime = testslice3.EndDateTime;
            var expected1 = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice1 } } };
            var expected2 = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice2, testslice3 } } };
            targetRespository.GetOriginalDataByDate(testslice1.StartDate)
                .Returns(expected1);
            targetRespository.GetOriginalDataByDate(testslice2.StartDate)
                .Returns(expected2);
            var expectedRoot = new CNRoot();
            expectedRoot.AddOrUpdateTask(testTask);
            rootDataRepository.GetRootData().Returns(expectedRoot);
            //Act
            targetService.DeleteTimeSlices(testTask,  testslice2 );

            //Assert
            targetRespository.Received().OverWriteToDataSourceByDate(testslice2.StartDate, Arg.Is<Dictionary<int, List<CNTimeSlice>>>(x => x.ContainsKey(testTask.TaskId) && !new List<CNTimeSlice>() { testslice3 }.Except(x[testTask.TaskId]).Any()));
            rootDataRepository.DidNotReceive().Persistence(Arg.Any<CNRoot>());
        }
        [Test]
        public void DeleteATimeSlice_ChangingTaskStartEndTime()
        {
            //Arrange
            var rootDataRepository = Substitute.For<IRootDataProvider>();
            var targetRespository = Substitute.For<ITimeSliceProvider>();
            TimeSliceService targetService = new TimeSliceService(targetRespository, rootDataRepository);
            var testTask = DomainTestHelper.GetARandomTask(1);
            var currentTime = DateTime.Now;
            var testslice1 = new CNTimeSlice(currentTime.AddDays(-2).AddMinutes(-3), currentTime.AddDays(-1).AddMinutes(-2));
            var testslice2 = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-2), currentTime.AddDays(-1).AddMinutes(-1));
            var testslice3 = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-1), currentTime.AddDays(-1).AddMinutes(2));
            testTask.StartTime = testslice1.StartDateTime;
            testTask.EndTime = testslice3.EndDateTime;
            var expected1 = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice1 } } };
            var expected2 = new Dictionary<int, List<CNTimeSlice>> { { testTask.TaskId, new List<CNTimeSlice>() { testslice2, testslice3 } } };
            targetRespository.GetOriginalDataByDate(testslice1.StartDate)
                .Returns(expected1);
            targetRespository.GetOriginalDataByDate(testslice2.StartDate)
                .Returns(expected2);
            var expectedRoot = new CNRoot();
            expectedRoot.AddOrUpdateTask(testTask);
            rootDataRepository.GetRootData().Returns(expectedRoot);
            //Act
            targetService.DeleteTimeSlices(testTask,  testslice3 );

            //Assert
            targetRespository.Received().OverWriteToDataSourceByDate(testslice2.StartDate, Arg.Is<Dictionary<int, List<CNTimeSlice>>>(x => x.ContainsKey(testTask.TaskId) && !new List<CNTimeSlice>() { testslice2 }.Except(x[testTask.TaskId]).Any()));
            rootDataRepository.Received().Persistence(Arg.Is<CNRoot>(x => x.GetTaskById(testTask.TaskId).StartTime == testTask.StartTime &&
                                                                          x.GetTaskById(testTask.TaskId).EndTime == testslice2.EndDateTime));
        }
    }
}
