using System.Collections.Generic;
using XOutput.Devices.Mapper;

namespace XOutput.Devices.Controller
{
    public class ControllerConfig<T>
    {
        public string DisplayName { get; set; }
        private Dictionary<T, InputMapperCollection> mapping = new Dictionary<T, InputMapperCollection>();
        public Dictionary<T, InputMapperCollection> InputMapping
        {
            get => mapping;
            set
            {
                if (value != mapping && value != null)
                {
                    mapping = value;
                }
            }
        }
        private List<ForceFeedbackMapper> forceFeedback = new List<ForceFeedbackMapper>();
        public List<ForceFeedbackMapper> ForceFeedbackMapping
        {
            get => forceFeedback;
            set
            {
                if (value != forceFeedback && value != null)
                {
                    forceFeedback = value;
                }
            }
        }
    }
}
