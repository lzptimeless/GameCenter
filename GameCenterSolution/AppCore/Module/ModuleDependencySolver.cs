// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace AppCore
{
    /// <summary>
    /// Used by <see cref="ModuleInitializer"/> to get the load sequence
    /// for the modules to load according to their dependencies.
    /// </summary>
    internal class ModuleDependencySolver
    {
        private readonly ListDictionary<string, string> _dependencyMatrix = new ListDictionary<string, string>();
        private readonly List<string> _knownModules = new List<string>();

        /// <summary>
        /// Adds a module to the solver.
        /// </summary>
        /// <param name="name">The name that uniquely identifies the module.</param>
        public void AddModule(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new StringNullOrEmptyException("name");

            AddToDependencyMatrix(name);
            AddToKnownModules(name);
        }

        /// <summary>
        /// Adds a module dependency between the modules specified by dependingModule and
        /// dependentModule.
        /// </summary>
        /// <param name="dependingModule">The name of the module with the dependency.</param>
        /// <param name="dependentModule">The name of the module dependingModule
        /// depends on.</param>
        public void AddDependency(string dependingModule, string dependentModule)
        {
            if (String.IsNullOrEmpty(dependingModule))
                throw new StringNullOrEmptyException("dependingModule");

            if (String.IsNullOrEmpty(dependentModule))
                throw new StringNullOrEmptyException("dependentModule");

            if (!_knownModules.Contains(dependingModule))
                throw new ArgumentException($"Unkown module: {dependingModule}", "dependingModule");

            AddToDependencyMatrix(dependentModule);
            _dependencyMatrix.Add(dependentModule, dependingModule);
        }

        private void AddToDependencyMatrix(string module)
        {
            if (!_dependencyMatrix.ContainsKey(module))
            {
                _dependencyMatrix.Add(module);
            }
        }

        private void AddToKnownModules(string module)
        {
            if (!_knownModules.Contains(module))
            {
                _knownModules.Add(module);
            }
        }

        /// <summary>
        /// Calculates an ordered vector according to the defined dependencies.
        /// Non-dependant modules appears at the beginning of the resulting array.
        /// </summary>
        /// <returns>The resulting ordered list of modules.</returns>
        /// <exception cref="CyclicDependencyFoundException">This exception is thrown
        /// when a cycle is found in the defined depedency graph.</exception>
        public string[] Solve()
        {
            List<string> skip = new List<string>();
            while (skip.Count < _dependencyMatrix.Count)
            {
                List<string> leaves = this.FindLeaves(skip);
                if (leaves.Count == 0 && skip.Count < _dependencyMatrix.Count)
                {
                    throw new CyclicDependencyFoundException();
                }
                skip.AddRange(leaves);
            }
            skip.Reverse();

            if (skip.Count > _knownModules.Count)
            {
                var moduleNames = this.FindMissingModules(skip);
                throw new MissingModulesException(moduleNames);
            }

            return skip.ToArray();
        }

        private List<string> FindMissingModules(List<string> skip)
        {
            List<string> missingModules = new List<string>();

            foreach (string module in skip)
            {
                if (!_knownModules.Contains(module))
                {
                    missingModules.Add(module);
                }
            }

            return missingModules;
        }

        /// <summary>
        /// Gets the number of modules added to the solver.
        /// </summary>
        /// <value>The number of modules.</value>
        public int ModuleCount
        {
            get { return _dependencyMatrix.Count; }
        }

        private List<string> FindLeaves(List<string> skip)
        {
            List<string> result = new List<string>();

            foreach (string precedent in _dependencyMatrix.Keys)
            {
                if (skip.Contains(precedent))
                {
                    continue;
                }

                int count = 0;
                foreach (string dependent in _dependencyMatrix[precedent])
                {
                    if (skip.Contains(dependent))
                    {
                        continue;
                    }
                    count++;
                }
                if (count == 0)
                {
                    result.Add(precedent);
                }
            }
            return result;
        }
    }
}