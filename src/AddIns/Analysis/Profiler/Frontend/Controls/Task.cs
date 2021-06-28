﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Threading;
using System.Windows.Threading;

namespace ICSharpCode.Profiler.Controls
{
	/// <summary>
	/// A task that is run in the background and can be cancelled.
	/// </summary>
	/// <example>
	/// Usage:
	/// <code>Task t = Task.Start(TaskMethod);
	/// t.RunWhenComplete(CompleteCallback);</code>
	/// Tasks can be cancelled:
	/// <code>t.Cancel();</code>
	/// The task method must poll for being cancelled:
	/// <code>if (Task.Current.IsCancelled) return;</code>
	/// </example>
	sealed class Task
	{
		[ThreadStatic] static Task currentTask;
		
		readonly object lockObj = new object();
		Action action;
		
		volatile bool cancel;
		bool isComplete;
		Action onCompleteActions;
		
		Task(Action action)
		{
			this.action = action;
		}
		
		public static Task Start(Action action)
		{
			if (action == null)
				throw new ArgumentNullException("action");
			Task t = new Task(action);
			ThreadPool.QueueUserWorkItem(t.Run, null);
			return t;
		}
		
		/// <summary>
		/// Gets the task running on the current thread.
		/// </summary>
		public static Task Current {
			get {
				return currentTask;
			}
		}
		
		void Run(object state)
		{
			currentTask = this;
			try {
				action();
			} finally {
				currentTask = null;
				// let the GC collect the action delegate and any objects referenced by its closure
				action = null;
			}
			
			lock (lockObj) {
				isComplete = true;
			}
			// The lock above ensures that now onCompleteActions is not modified anymore.
			if (onCompleteActions != null) {
				onCompleteActions();
				// let the GC collect the onCompleteActions and objects referenced by their closures
				onCompleteActions = null;
			}
		}
		
		/// <summary>
		/// Gets whether the task was cancelled.
		/// </summary>
		public bool IsCancelled {
			get { return cancel; }
		}
		
		/// <summary>
		/// Cancels the task (simply sets IsCancelled to true).
		/// If the task has already finished, the cancel flag is not changed.
		/// (this is done to ensure that IsCancelled cannot change asynchronously to the
		/// execution of "RunWhenComplete" callbacks)
		/// </summary>
		public void Cancel()
		{
			lock (lockObj) {
				if (!isComplete) {
					cancel = true;
				}
			}
		}
		
		/// <summary>
		/// Runs the action after the task has completed. If the task already has completed,
		/// the action is run immediately.
		/// The action will run on a thread pool thread (not necessarily on the same thread that executed the task).
		/// 
		/// When you call RunWhenComplete multiple times, it is possible that the completion actions will
		/// execute in parallel.
		/// </summary>
		public void RunWhenComplete(Action action)
		{
			if (action == null)
				throw new ArgumentNullException("action");
			lock (lockObj) {
				if (isComplete) {
					// already complete: start action immediately (but on another thread!)
					ThreadPool.QueueUserWorkItem(state => action());
				} else {
					// not yet complete: store action and run it when complete
					onCompleteActions += action;
				}
			}
		}
		
		/// <summary>
		/// Runs the action after the task has completed. If the task already has completed,
		/// the action is run immediately.
		/// The action will be run on the Dispatcher with normal priority.
		/// </summary>
		public void RunWhenComplete(Dispatcher dispatcher, Action action)
		{
			if (dispatcher == null)
				throw new ArgumentNullException("dispatcher");
			if (action == null)
				throw new ArgumentNullException("action");
			lock (lockObj) {
				if (isComplete) {
					// already complete: start action immediately
					dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
				} else {
					// not yet complete: store action and run it when complete
					onCompleteActions += delegate {
						dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
					};
				}
			}
		}
	}
}
