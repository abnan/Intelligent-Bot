using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace deploybot
{

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://opensearch.org/searchsuggest2")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://opensearch.org/searchsuggest2", IsNullable = false)]
    public partial class SearchSuggestion
    {

        private SearchSuggestionQuery queryField;

        private SearchSuggestionSection sectionField;

        private decimal versionField;

        /// <remarks/>
        public SearchSuggestionQuery Query
        {
            get
            {
                return this.queryField;
            }
            set
            {
                this.queryField = value;
            }
        }

        /// <remarks/>
        public SearchSuggestionSection Section
        {
            get
            {
                return this.sectionField;
            }
            set
            {
                this.sectionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://opensearch.org/searchsuggest2")]
    public partial class SearchSuggestionQuery
    {

        private string spaceField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string space
        {
            get
            {
                return this.spaceField;
            }
            set
            {
                this.spaceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://opensearch.org/searchsuggest2")]
    public partial class SearchSuggestionSection
    {

        private SearchSuggestionSectionItem itemField;

        /// <remarks/>
        public SearchSuggestionSectionItem Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://opensearch.org/searchsuggest2")]
    public partial class SearchSuggestionSectionItem
    {

        private SearchSuggestionSectionItemText textField;

        private SearchSuggestionSectionItemUrl urlField;

        private SearchSuggestionSectionItemDescription descriptionField;

        private SearchSuggestionSectionItemImage imageField;

        /// <remarks/>
        public SearchSuggestionSectionItemText Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        public SearchSuggestionSectionItemUrl Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        public SearchSuggestionSectionItemDescription Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        public SearchSuggestionSectionItemImage Image
        {
            get
            {
                return this.imageField;
            }
            set
            {
                this.imageField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://opensearch.org/searchsuggest2")]
    public partial class SearchSuggestionSectionItemText
    {

        private string spaceField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string space
        {
            get
            {
                return this.spaceField;
            }
            set
            {
                this.spaceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://opensearch.org/searchsuggest2")]
    public partial class SearchSuggestionSectionItemUrl
    {

        private string spaceField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string space
        {
            get
            {
                return this.spaceField;
            }
            set
            {
                this.spaceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://opensearch.org/searchsuggest2")]
    public partial class SearchSuggestionSectionItemDescription
    {

        private string spaceField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string space
        {
            get
            {
                return this.spaceField;
            }
            set
            {
                this.spaceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://opensearch.org/searchsuggest2")]
    public partial class SearchSuggestionSectionItemImage
    {

        private string sourceField;

        private byte widthField;

        private byte heightField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string source
        {
            get
            {
                return this.sourceField;
            }
            set
            {
                this.sourceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte width
        {
            get
            {
                return this.widthField;
            }
            set
            {
                this.widthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte height
        {
            get
            {
                return this.heightField;
            }
            set
            {
                this.heightField = value;
            }
        }
    }


}