using Google.Cloud.Firestore;

namespace CrudFirebase.Models
{
    [FirestoreData]
    public class Pessoa
    {
        public string PessoaId { get; set; }

        [FirestoreProperty]
        public string Nome { get; set; }

        [FirestoreProperty]
        public int Idade { get; set; }
    }
}
