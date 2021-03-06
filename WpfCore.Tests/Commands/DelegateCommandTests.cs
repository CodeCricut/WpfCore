using NUnit.Framework;
using System;
using WpfCore.Commands;

namespace WpfCore.Tests.Commands
{
	public class DelegateCommandTests
	{
		private bool _executeCalled;
		private object _executeParamPassed;
		private bool _canExecuteCalled;
		private object _canExecuteParamPassed;
		private bool _canExecuteChanged_Invoked;

		[Test]
		public void Constructor_ValidExecute_NullCanExecute_Creates()
		{
			var validDelegateCmd = CreateCommand_With_Execute_NullCanExecute();

			Assert.NotNull(validDelegateCmd);
		}

		[Test]
		public void Constructor_NullExecute_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => CreateCommandWithNullExecute());
		}

		[Test]
		public void Execute_Param_ExecutesWithParam()
		{
			var cmd = CreateCommand_With_Execute_NullCanExecute();
			var param = 20;

			cmd.Execute(param);

			Assert.True(_executeCalled);
			Assert.AreEqual(param, _executeParamPassed);
		}

		[Test]
		public void Execute_Default_ExecutesWithNull()
		{
			var cmd = CreateCommand_With_Execute_NullCanExecute();

			cmd.Execute();

			Assert.True(_executeCalled);
			Assert.IsNull(_executeParamPassed);
		}

		[Test]
		public void CanExecute_DefaultParam_CanExecuteFuncProvided_ReturnsCanExecuteNull()
		{
			var cmd = CreateCommandWithExecuteAndCanExecute();

			var canExecute = cmd.CanExecute();

			Assert.True(_canExecuteCalled);
			Assert.IsNull(_canExecuteParamPassed);
			Assert.AreEqual(CanExecute_True(null), canExecute);
		}

		[Test]
		public void CanExecute_Param_CanExecuteFuncProvided_ReturnsCanExecuteParam()
		{
			var cmd = CreateCommandWithExecuteAndCanExecute();
			var param = "hello";

			var canExecute = cmd.CanExecute(param);

			Assert.True(_canExecuteCalled);
			Assert.AreEqual(param, _canExecuteParamPassed);
			Assert.AreEqual(CanExecute_True(param), canExecute);
		}

		[Test]
		public void CanExecute_NullCanExecuteFunc_ReturnsTrue()
		{
			var cmd = CreateCommand_With_Execute_NullCanExecute();

			var canExecute = cmd.CanExecute();

			Assert.IsTrue(canExecute);
		}

		[Test]
		public void RaiseCanExecuteChanged_TriggersCanExecuteChanged()
		{
			var cmd = CreateCommand_With_Execute_NullCanExecute();
			cmd.CanExecuteChanged += Cmd_CanExecuteChanged;

			cmd.RaiseCanExecuteChanged();

			Assert.True(_canExecuteChanged_Invoked);
		}

		[Test]
		public void NoAction_CanExecuteChanged_NotTriggered()
		{
			var cmd = CreateCommand_With_Execute_NullCanExecute();
			cmd.CanExecuteChanged += Cmd_CanExecuteChanged;

			// No action

			Assert.False(_canExecuteChanged_Invoked);
		}

		private DelegateCommand CreateCommand_With_Execute_NullCanExecute()
		{
			return new DelegateCommand(Execute);
		}

		private static DelegateCommand CreateCommandWithNullExecute()
		{
			return new DelegateCommand(null);
		}

		private DelegateCommand CreateCommandWithExecuteAndCanExecute()
		{
			return new DelegateCommand(Execute, CanExecute_True);
		}

		private void Execute(object param)
		{
			_executeCalled = true;
			_executeParamPassed = param;
		}

		private bool CanExecute_True(object param)
		{
			_canExecuteCalled = true;
			_canExecuteParamPassed = param;

			return true;
		}

		private void Cmd_CanExecuteChanged(object sender, EventArgs e)
		{
			_canExecuteChanged_Invoked = true;
		}
	}
}
