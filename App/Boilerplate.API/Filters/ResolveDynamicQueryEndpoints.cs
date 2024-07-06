using System.Reflection;

using DynamicQueryBuilder;

using Boilerplate.API.Attributes;

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Boilerplate.API.Filters
{
    internal sealed class ResolveDynamicQueryEndpoints : IOperationFilter
    {
        private readonly string _description;
        private readonly string _dqbResolveParam;
        private IReadOnlyList<string> _dqbParameterNames;

        public ResolveDynamicQueryEndpoints(
            string dqbResolveParam = "",
            string description = "DynamicQuery")
        {
            _description = description;
            _dqbResolveParam = dqbResolveParam;
            _dqbParameterNames = new List<string>
            {
                "Filters",
                "SortOptions",
                "PaginationOption.Count",
                "PaginationOption.Offset",
                "PaginationOption.DataSetCount",
                "PaginationOption.AssignDataSetCount",
                "UsesCaseInsensitiveSource",
                "IgnorePredefinedOrders"
            };
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context
                .MethodInfo
                .GetCustomAttribute<DynamicQueryAttribute>() != null)
            {
                OpenApiSchema apiSchema;
                if (operation.Parameters != null)
                {
                    var otherParams = operation.Parameters.Where(p => !_dqbParameterNames.Contains(p.Name)).ToList();

                    operation.Parameters.Clear();

                    foreach (var otherParam in otherParams)
                    {
                        operation.Parameters.Add(otherParam);
                    }

                    apiSchema = context.SchemaGenerator.GenerateSchema(typeof(string), context.SchemaRepository);
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        In = ParameterLocation.Query,
                        Schema = apiSchema,
                        Description = _description,
                        Name = _dqbResolveParam
                    });

                    var methodParams = context.MethodInfo.GetParameters();

                    foreach (var methodParam in methodParams)
                    {
                        if (methodParam.GetCustomAttribute<SwaggerIncludeAttribute>() != null)
                        {
                            apiSchema = context.SchemaGenerator.GenerateSchema(methodParam.ParameterType, context.SchemaRepository);
                            if (context.ApiDescription.ActionDescriptor.Parameters.Where(p => p.Name == methodParam.Name).Any(p => p.BindingInfo.BindingSource.DisplayName == "Path"))
                            {
                                operation.Parameters.Add(new OpenApiParameter
                                {
                                    In = ParameterLocation.Path,
                                    Name = methodParam.Name,
                                    Required = true,
                                    Schema = apiSchema
                                });
                            }
                            else
                            {
                                operation.Parameters.Add(new OpenApiParameter
                                {
                                    In = ParameterLocation.Query,
                                    Name = methodParam.Name,
                                    Required = true,
                                    Schema = apiSchema
                                });
                            }
                        }
                    }
                }
            }
        }
    }
}
