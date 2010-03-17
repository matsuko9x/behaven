// <copyright file="PlainTextReporter.cs" company="Jason Diamond">
//
// Copyright (c) 2009-2010 Jason Diamond
//
// This source code is released under the MIT License.
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies
// of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
//
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BehaveN
{
    /// <summary>
    /// Represents a plain text reporter.
    /// </summary>
    public class PlainTextReporter : Reporter
    {
        /// <summary>
        /// The symbol for an undefined step.
        /// </summary>
        public const string Undefined = "?";

        /// <summary>
        /// The symbol for a pending step.
        /// </summary>
        public const string Pending = "*";

        /// <summary>
        /// The symbol for a passed step.
        /// </summary>
        public const string Passed = " ";

        /// <summary>
        /// The symbol for a failed step.
        /// </summary>
        public const string Failed = "!";

        /// <summary>
        /// The symbol for a skipped step.
        /// </summary>
        public const string Skipped = "-";

        /// <summary>
        /// Initializes a new instance of the <see cref="PlainTextReporter"/> class.
        /// </summary>
        public PlainTextReporter() : this(Console.Out)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlainTextReporter"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public PlainTextReporter(TextWriter writer)
        {
            _writer = writer;
        }

        private TextWriter _writer;
        private StepType _lastStepType;

        /// <summary>
        /// Reports the specifications file.
        /// </summary>
        /// <param name="specificationsFile">The specifications file.</param>
        /// <remarks>This reports all scenarios in the file and their
        /// undefined steps.</remarks>
        public override void ReportSpecificationsFile(SpecificationsFile specificationsFile)
        {
            foreach (Scenario scenario in specificationsFile.Scenarios)
            {
                ReportScenario(scenario);

                WriteDivider();
            }

            ReportUndefinedSteps(specificationsFile.GetUndefinedSteps());
        }

        /// <summary>
        /// Reports the scenario.
        /// </summary>
        /// <param name="scenario">The scenario.</param>
        public override void ReportScenario(Scenario scenario)
        {
            _writer.WriteLine("Scenario: {0}", scenario.Name);
            _writer.WriteLine();

            foreach (Step step in scenario.Steps)
            {
                switch (step.Result)
                {
                    case StepResult.Passed:
                        ReportPassed(step);
                        break;
                    case StepResult.Failed:
                        ReportFailed(step);
                        break;
                    case StepResult.Undefined:
                        ReportUndefined(step);
                        break;
                    case StepResult.Pending:
                        ReportPending(step);
                        break;
                    case StepResult.Skipped:
                        ReportSkipped(step);
                        break;
                }

                if (step.Block != null)
                    ReportBlock(step.Block);
            }

            _writer.WriteLine();

            ReportException(scenario);
        }

        private void WriteDivider()
        {
            _writer.WriteLine("---");
            _writer.WriteLine();
        }

        private void ReportException(Scenario scenario)
        {
            if (scenario.Exception != null)
            {
                _writer.WriteLine(scenario.Exception.Message);

                if (!string.IsNullOrEmpty(scenario.Exception.StackTrace))
                {
                    _writer.WriteLine();
                    _writer.WriteLine(GetStackTraceThatIsClickableInOutputWindow(scenario.Exception));
                }

                _writer.WriteLine();
            }
        }

        private string GetStackTraceThatIsClickableInOutputWindow(Exception e)
        {
            return Regex.Replace(e.StackTrace, @"  at (.+) in (.+):line (\d+)", "$2($3): $1");
        }

        /// <summary>
        /// Reports the undefined steps.
        /// </summary>
        /// <param name="undefinedSteps">The undefined steps.</param>
        public override void ReportUndefinedSteps(ICollection<Step> undefinedSteps)
        {
            if (undefinedSteps.Count > 0)
            {
                _writer.WriteLine("Your undefined steps can be defined with the following code:");
                _writer.WriteLine();

                foreach (Step undefinedStep in undefinedSteps)
                {
                    ReportUndefinedStep(undefinedStep);
                }
            }
        }

        private void ReportUndefined(Step step)
        {
            ReportStatus(step, Undefined);
        }

        private void ReportPending(Step step)
        {
            ReportStatus(step, Pending);
        }

        private void ReportPassed(Step step)
        {
            ReportStatus(step, Passed);
        }

        private void ReportFailed(Step step)
        {
            ReportStatus(step, Failed);
        }

        private void ReportSkipped(Step step)
        {
            ReportStatus(step, Skipped);
        }

        private void ReportStatus(Step step, string status)
        {
            if (_lastStepType != StepType.Unknown && step.Type != _lastStepType)
            {
                _writer.WriteLine();
            }

            _writer.WriteLine(status + " " + step.Text);

            _lastStepType = step.Type;
        }

        private void ReportBlock(IBlock block)
        {
            _writer.Write(block.Format());
        }

        private void ReportUndefinedStep(Step undefinedStep)
        {
            string code = UndefinedStepDefinitionHelper.GetUndefinedStepCode(undefinedStep);
            _writer.WriteLine(code);
            _writer.WriteLine();
        }
    }
}
