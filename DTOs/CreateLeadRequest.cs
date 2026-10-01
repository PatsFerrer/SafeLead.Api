using System.ComponentModel.DataAnnotations;

namespace SafeLead.Api.DTOs
{
    public class CreateLeadRequest
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
        public string Name { get; init; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        [StringLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres.")]
        public string Email { get; init; } = string.Empty;

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [RegularExpression(@"^\+?[1-9]\d{9,14}$", ErrorMessage = "Telefone inválido. Use apenas números com DDD.")]
        public string Phone { get; init; } = string.Empty;

        [Required(ErrorMessage = "A mensagem é obrigatória.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "A mensagem deve ter entre 10 e 1000 caracteres.")]
        public string Message { get; init; } = string.Empty;
    }
}
