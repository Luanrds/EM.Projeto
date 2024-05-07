﻿using EM.Domain;
using EM.Domain.Interfaces;
using EM.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EM.Web.Controllers
{
	public class AlunoController : Controller
	{
		private readonly IRepositorioAbstrato<Aluno> _repositorioAlunoAbstratro;
		private readonly IRepositorioAbstrato<Cidade> _repositorioCidade;
		private readonly IRepositorioAluno<Aluno> _repositorioAluno;

		public AlunoController(IRepositorioAbstrato<Aluno> repositorioAluno, IRepositorioAbstrato<Cidade> repositorioCidade, IRepositorioAluno<Aluno> repositorioRemove)
		{
			_repositorioAlunoAbstratro = repositorioAluno;
			_repositorioCidade = repositorioCidade;
			_repositorioAluno = repositorioRemove;
		}


		public IActionResult CadastrarAluno(int? id)
		{
			ViewBag.Cidades = _repositorioCidade.GetAll().ToList();
			if(id != null)
			{
				Aluno? aluno = _repositorioAlunoAbstratro.Get(c => c.Matricula == id).FirstOrDefault();
				
				ViewBag.IsEdicao = true;
				return View(aluno);
			}
			ViewBag.IsEdicao = false;
			return View(new Aluno());

		}

		[HttpPost]
		public IActionResult CadastrarAluno(Aluno aluno)
		{
			if (ModelState.IsValid)
			{
				if(aluno.Matricula > 0)
				{
					_repositorioAlunoAbstratro.Update(aluno);
				}
				else
				{
					_repositorioAlunoAbstratro.Add(aluno);
				}

				return RedirectToAction("Index", "Home");
			}
			ViewBag.IsEdicao = aluno.Matricula > 0;
			ViewBag.Cidades = _repositorioCidade.GetAll().ToList();
			return View(aluno);
		}

		[HttpPost]
		public IActionResult RemoveAluno(Aluno aluno)
		{
			_repositorioAluno.Remove(aluno);
			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public ActionResult Search(string searchTerm, string searchType)
		{
			if (searchType != null) // Verifica se searchType não é nulo
			{
				if (searchType.ToLower() == "matricula" && int.TryParse(searchTerm, out int matricula))
				{
					Aluno aluno = _repositorioAluno.GetByMatricula(matricula);
					Console.WriteLine($"Aluno encontrado: {aluno}"); // Adiciona esta linha para depuração
					IEnumerable<Aluno> alunos = aluno != null ? new List<Aluno> { aluno } : new List<Aluno>();
					return View("Views/Home/Index.cshtml", alunos);
				}
				else if (searchType.ToLower() == "nome")
				{
					IEnumerable<Aluno> alunos = _repositorioAluno.GetByContendoNoNome(searchTerm);
					return View("Views/Home/Index.cshtml", alunos);
				}
			}

			// Se searchType for nulo ou não corresponder a "matricula" ou "nome", retorna uma visualização vazia
			return View("Views/Home/Index.cshtml", new List<Aluno>());
		}

	}
}
