using System.Net.Mime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using levitas.PoContract;

namespace levitas.CadastroDeAlunos;
public class AlunoLoadMetaData : PoDynamicEditMetaData
{
    public AlunoLoadMetaData()
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
                    Visible = false,
                    Type = PoDynamicFieldType.text
                },
                new PoDynamicField()
                {
                    Property = "nome",
                    Label = "Nome",
                    GridColumns= 6,
                    Filter = true,
                    Type = PoDynamicFieldType.text
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
                    Visible = false,
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
                    GridColumns= 6,
                    Type = PoDynamicFieldType.text,
                    Filter = true
                },
                new PoDynamicField()
                {
                    Property = "temSkate",
                    Label = "Tem Skate",
                    Type = PoDynamicFieldType.boolean
                },
                new PoDynamicField()
                {
                    Property = "urlFoto",
                    Label = "Foto",
                    Visible = false
                },
                new PoDynamicField()
                {
                    Property = "urlTermoDeResponsabilidadeAssinado",
                    Label = "Termo de Responsabilidade Assinado",
                    Visible = false,
                    Type = PoDynamicFieldType.text
                },
            };

    }
}