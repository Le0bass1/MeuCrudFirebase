using CrudFirebase.Models;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CrudFirebase.Controllers
{
    public class PessoasController : Controller
    {
        public FirestoreDb _firestoreDb;

        public PessoasController()
        {
            // Realizado a autenticacao via CLI, pois via Json, nao estava sendo possivel.

            //var diretorio = @"Diretorio do arquivo JSON";
            //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", diretorio);
            _firestoreDb = FirestoreDb.Create("ID do projeto");
        }
        public async Task<IActionResult> Index()
        {
            Query pessoasQuery = _firestoreDb.Collection("pessoas");
            QuerySnapshot pessoasQuerySnapshot = await pessoasQuery.GetSnapshotAsync();
            List<Pessoa> listaPessoas = new List<Pessoa>();

            foreach(DocumentSnapshot documentSnapshot in pessoasQuerySnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    Dictionary<string, object> pessoa = documentSnapshot.ToDictionary();
                    string json = JsonConvert.SerializeObject(pessoa);
                    Pessoa novaPessoa = JsonConvert.DeserializeObject<Pessoa>(json);
                    novaPessoa.PessoaId = documentSnapshot.Id;
                    listaPessoas.Add(novaPessoa);
                }
            };

            return View(listaPessoas);
        }

        [HttpGet]
        public IActionResult NovaPessoa()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NovaPessoa(Pessoa pessoa)
        {
            CollectionReference collectionReference = _firestoreDb.Collection("pessoas");
            await collectionReference.AddAsync(pessoa);
            return RedirectToAction(nameof (Index));
        }

        [HttpGet]
        public async Task<IActionResult> AtualizarPessoa(string pessoaId)
        {
            DocumentReference documentReference = _firestoreDb.Collection("pessoas").Document(pessoaId);
            DocumentSnapshot documentSnapshot = await documentReference.GetSnapshotAsync();

            if(documentSnapshot.Exists)
            {
                Pessoa pessoa = documentSnapshot.ConvertTo<Pessoa>();
                return View(pessoa);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AtualizarPessoa(Pessoa pessoa)
        {
            DocumentReference documentReference = _firestoreDb.Collection("pessoas").Document(pessoa.PessoaId);
            await documentReference.SetAsync(pessoa, SetOptions.Overwrite);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ExcluirPessoa(string pessoaId)
        {
            DocumentReference documentReference = _firestoreDb.Collection("pessoas").Document(pessoaId);
            await documentReference.DeleteAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
