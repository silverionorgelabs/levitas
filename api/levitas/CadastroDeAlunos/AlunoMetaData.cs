using System.Collections.Generic;
using levitas.PoContract;

namespace levitas.CadastroDeAlunos
{
    public class AlunoMetaData : PoDynamicMetaData
    {
        
        public AlunoMetaData()
        {
            Title = "Cadastro de Alunos";
            Version = 1;
            Fields = new List<PoDynamicField>()
            {
                new PoDynamicField()
                {
                    Key = true,
                    Property = "id",
                    Label = "Id",
                    Type = PoDynamicFieldType.text
                },
                new PoDynamicField()
                {
                    Property = "nome",
                    Label = "Nome",
                    Type = PoDynamicFieldType.text,
                    Filter = true
                },
                new PoDynamicField()
                {
                    Property = "dataDeNascimento",
                    Label = "Data de Nascimento",
                    Type = PoDynamicFieldType.date
                },
                new PoDynamicField()
                {
                    Property = "idade",
                    Label = "Idade",
                    Type = PoDynamicFieldType.number
                },
                new PoDynamicField()
                {
                    Property = "telefone",
                    Label = "Telefone",
                    Type = PoDynamicFieldType.text
                },
                new PoDynamicField()
                {
                    Property = "nomeDoResponsavel",
                    Label = "Nome do Responsável",
                    Type = PoDynamicFieldType.text,
                    Filter = true
                },
                new PoDynamicField()
                {
                    Property = "temSkate",
                    Label = "Tem Skate",
                    Type = PoDynamicFieldType.boolean,
                    
                },
                new PoDynamicField()
                {
                    Property = "urlFoto",
                    Label = "Foto",
                    Type = PoDynamicFieldType.text
                },
                new PoDynamicField()
                {
                    Property = "urlTermoDeResponsabilidadeAssinado",
                    Label = "Termo de Responsabilidade Assinado",
                    Type = PoDynamicFieldType.text
                },
            };

        }
    }
}
