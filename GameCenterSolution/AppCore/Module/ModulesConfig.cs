using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore
{
    public class ModulesConfigSection : ConfigurationSection
    {
        static ModulesConfigSection _cache;

        [ConfigurationProperty("", IsDefaultCollection = true, IsKey = false)]
        public ModulesConfigElementCollection Modules
        {
            get { return (ModulesConfigElementCollection)this[""]; }
            set { this[""] = value; }
        }

        public static ModulesConfigSection GetConfig()
        {
            if (_cache == null)
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var section = config.GetSection("modules") as ModulesConfigSection;
                Interlocked.CompareExchange(ref _cache, section, null);
            }

            return _cache;
        }
    }

    [ConfigurationCollection(typeof(ModulesConfigElementCollection))]
    public class ModulesConfigElementCollection : ConfigurationElementCollection
    {
        public ModulesConfigElementCollection()
        { }

        public ModulesConfigElementCollection(ModuleConfigElement[] modules)
        {
            if (modules == null) throw new System.ArgumentNullException("modules");
            foreach (ModuleConfigElement module in modules)
            {
                BaseAdd(module);
            }
        }

        protected override bool ThrowOnDuplicate
        {
            get { return false; }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return "module"; }
        }

        public ModuleConfigElement this[int index]
        {
            get { return (ModuleConfigElement)base.BaseGet(index); }
        }

        public void Add(ModuleConfigElement module)
        {
            BaseAdd(module);
        }

        public bool Contains(string name)
        {
            return base.BaseGet(name) != null;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ModuleConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ModuleConfigElement)element).Name;
        }
    }

    public class ModuleConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("file", IsRequired = true)]
        public string File
        {
            get { return (string)this["file"]; }
            set { this["file"] = value; }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("", IsDefaultCollection = true, IsKey = false)]
        public ModuleDependenciesConfigElementCollection Dependencies
        {
            get { return (ModuleDependenciesConfigElementCollection)this[""]; }
            set { this[""] = value; }
        }
    }

    [ConfigurationCollection(typeof(ModuleDependenciesConfigElementCollection))]
    public class ModuleDependenciesConfigElementCollection : ConfigurationElementCollection
    {
        public ModuleDependenciesConfigElementCollection()
        { }

        public ModuleDependenciesConfigElementCollection(ModuleDependencyConfigElement[] dependencies)
        {
            if (dependencies == null) throw new System.ArgumentNullException("dependencies");
            foreach (ModuleDependencyConfigElement dependency in dependencies)
            {
                BaseAdd(dependency);
            }
        }

        protected override bool ThrowOnDuplicate
        {
            get { return false; }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return "dependency"; }
        }

        public ModuleDependencyConfigElement this[int index]
        {
            get { return (ModuleDependencyConfigElement)base.BaseGet(index); }
        }

        public void Add(ModuleDependencyConfigElement module)
        {
            BaseAdd(module);
        }

        public bool Contains(string name)
        {
            return base.BaseGet(name) != null;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ModuleDependencyConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ModuleDependencyConfigElement)element).Name;
        }
    }

    public class ModuleDependencyConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }
    }
}
