using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookstore.Models {
	public class ApplicationUser : IdentityUser {
		
		[Required]
		public string Name { get; set; }
		public string? StreetAddress { get; set; }
		public string? City { get; set; }
		public string? State { get; set; }
		public string? PostalCode { get; set; }
		[ForeignKey("CompanyId")]
		[ValidateNever]
		public int? CompanyId { get; set; }

	}
}
