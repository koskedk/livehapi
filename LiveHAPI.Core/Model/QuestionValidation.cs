﻿using System;
using System.Collections.Generic;
using System.Linq;
using LiveHAPI.Shared.Custom;
using LiveHAPI.Shared.Model;

namespace LiveHAPI.Core.Model
{
    public class QuestionValidation : Entity<Guid>
    {
        
        public string ValidatorId { get; set; }
        
        public string ValidatorTypeId { get; set; }
        public int Revision { get; set; }
        public string MinLimit { get; set; }
        public string MaxLimit { get; set; }
        
        public Guid QuestionId { get; set; }

        public QuestionValidation()
        {
            Id = LiveGuid.NewGuid();
        }

        public bool Evaluate(Response response)
        {
            var validationMessages=new List<string>();

            bool isValid = true;
            
            //  Required | Range

            if (ValidatorId.ToLower() == "Required".ToLower())
            {
                isValid = response.Obs.HasValue(response.Question.Concept.ConceptTypeId);

                if (!isValid)
                    validationMessages.Add($"Response is required");
            }
            
            if (isValid&& ValidatorId.ToLower() == "Range".ToLower())
            {
                bool isValidMin = true;
                bool isValidMax = true;

                if (ValidatorTypeId.ToLower() == "Numeric".ToLower())
                {
                    var obsValue = response.Obs.ValueNumeric;

                    if (!string.IsNullOrWhiteSpace(MinLimit))
                    {
                        isValidMin= obsValue >= Convert.ToDecimal(MinLimit);
                        if (!isValidMin)
                            validationMessages.Add($"Response cannot be Less than {MinLimit}");
                    }

                    if (!string.IsNullOrWhiteSpace(MaxLimit))
                    {
                        isValidMax = obsValue <= Convert.ToDecimal(MaxLimit);
                        if (!isValidMax)
                            validationMessages.Add($"Response cannot be greater than {MaxLimit}");
                    }

                    isValid = isValidMin&& isValidMax;
                }
                if (ValidatorTypeId.ToLower() == "Count".ToLower())
                {
                    var value = response.Obs.ValueMultiCoded;
                    var obsValue = string.IsNullOrWhiteSpace(value) ? new string[0] : value.Split(',');

                    if (!string.IsNullOrWhiteSpace(MinLimit))
                    {
                        isValidMin = obsValue.Length>0 && obsValue.Length >= Convert.ToDecimal(MinLimit);
                        if (!isValidMin)
                            validationMessages.Add($"Response cannot be Less than {MinLimit}");
                    }

                    if (!string.IsNullOrWhiteSpace(MaxLimit))
                    {
                        isValidMax = obsValue.Length > 0 && obsValue.Length <= Convert.ToDecimal(MaxLimit);
                        if (!isValidMax)
                            validationMessages.Add($"Response cannot be greater than {MaxLimit}");
                    }

                    isValid = isValidMin && isValidMax;
                }
            }

            if(!isValid)
                throw new ArgumentException(validationMessages.First());

            return isValid;
        }

        public override string ToString()
        {
            //$@"{ValidatorId}{ValidatorTypeId.ToLower().Equals("None".ToLower()) ? string.Empty : $",{ValidatorTypeId}")}";

            var mainInfo = $@"{ValidatorId}{(ValidatorTypeId.ToLower().Equals("None".ToLower()) ? "": $" | {ValidatorTypeId}")}";
            var minInfo = string.IsNullOrWhiteSpace(MinLimit) ? string.Empty : $" >={MinLimit}";
            var maxInfo = string.IsNullOrWhiteSpace(MaxLimit) ? string.Empty : $" <={MaxLimit}";
            return $"{mainInfo} {minInfo} {maxInfo}  [{Revision}]";
        }      
    }
}