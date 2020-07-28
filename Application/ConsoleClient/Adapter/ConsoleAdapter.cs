using Application.ConsoleClient.Validation;
using Application.ConsoleClient.Validation.Model;
using Domain.Model;
using Domain.PrimaryPort;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Application.ConsoleClient.Adapter
{
    public class ConsoleAdapter
    {
        private IValidationService<IEnumerable<string>> _validationService;
        private IConversionService _conversionService;

        public ConsoleAdapter(IValidationService<IEnumerable<string>> validationService, IConversionService conversionService)
        {
            _validationService = validationService;
            _conversionService = conversionService;
        }

        public void Run(string filePath)
        {
            var allLines = File.ReadAllLines(filePath);

            List<List<string>> lineBlocks = GetLineBlocks(allLines);

            Parallel.ForEach(lineBlocks, (lines) =>
            {
                if (_validationService.IsValid(lines))
                {
                    var result = _conversionService.Convert(ToConversionRequest(lines), ToExchangesRates(lines));
                    if (result.IsSuccess)
                    {
                        Console.WriteLine(result.Amount);
                    }
                    else
                    {
                        Console.WriteLine(result.ErrorMessage);
                    }
                }
                else
                {
                    Console.WriteLine(ErrorMessage.InconsistentInputData);
                    Console.WriteLine($"the file {filePath} contains inconsistent datas.");
                }
            });
        }

        private static List<List<string>> GetLineBlocks(string[] allLines)
        {
            var lineBlocks = new List<List<string>>();
            var block = new List<string>();

            for (int counter=0;counter<allLines.Length;counter++)
            {
                if(counter==allLines.Length-1 && !string.IsNullOrWhiteSpace(allLines[counter]))
                {
                    lineBlocks.Add(block);
                    break;
                }
                if (String.IsNullOrWhiteSpace(allLines[counter]))
                {
                    lineBlocks.Add(block);
                    block = new List<string>();
                }
                else
                    block.Add(allLines[counter]);
            }

            return lineBlocks;
        }

        private ConversionRequest ToConversionRequest(IEnumerable<string> lines)
        {
            var crl = new LineConversionRequest(lines.First());
            return new ConversionRequest(crl.SourceCurrency, crl.TargetCurrency, crl.Amount);
        }

        private IEnumerable<ExchangeRate> ToExchangesRates(IEnumerable<string> lines)
        {
            var myLines = lines.Skip(2).Select(l => new LineExchangeRate(l));
            return myLines.Select(x => new ExchangeRate(x.SourceCurrency, x.TargetCurrency, x.Rate));

        }

      
    }
}
