using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement.Model;
using Amazon.SimpleSystemsManagement;
using Amazon;

namespace ImageBrowser.Infrastructure.Services;
public static class ParametersService
{
    public static async Task<List<Parameter>> RetrieveParametersWithDecryption(string parameterName)
    {

        var ssmClient = new AmazonSimpleSystemsManagementClient(RegionEndpoint.USEast1); // Change region as needed

        try
        {
            var request = new GetParametersByPathRequest
            {
                Path = parameterName,
                WithDecryption = true // Set to true if the parameter is encrypted
            };

            var response = await ssmClient.GetParametersByPathAsync(request);

            //if (response.Parameters.Count > 0)
            //{
            //    Console.WriteLine($"Parameter Value: {String.Join(", ", response.Parameters.Select(p => p.Value))}");
            //}
            //else
            //{
            //    Console.WriteLine($"Parameter '{parameterName}' not found.");
            //}

            return response.Parameters;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving parameter: {ex.Message}");
            throw new Exception("Something went wrong");
        }
    }
}
