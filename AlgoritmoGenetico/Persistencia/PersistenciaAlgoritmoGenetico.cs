﻿using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Persistencia
{
    public class PersistenciaAlgoritmoGenetico
    {
        private Random random = new Random();
        private readonly int qtdIndividuos = 16;
        private readonly DbContextAG _contexto;
        private Individuo individuo;
        public PersistenciaAlgoritmoGenetico(DbContextAG contexto)
        {
            _contexto = contexto;
        }

        public List<Individuo> AlgoritmoGenetico(string ano)
        {
            var arrayIndividuo = InicializarPopulação(ano);

            return arrayIndividuo;
        }

        public List<Individuo> InicializarPopulação(string ano)
        {
            List<Individuo> arrayIndividuo = new List<Individuo>();

            var horariosBanco = getHorarios(ano);

            for (int i = 0; i < qtdIndividuos; i++)
            {
                individuo = CriarIndividuo(horariosBanco, ano);

                arrayIndividuo.Add(individuo);
            }
            return arrayIndividuo;
        }

        private Individuo CriarIndividuo(List<Horario> horarios, string ano)
        {
            // variaveis para auxiliar o sorteador
            int periodo1_2 = 0;
            int periodo3_4 = 0;
            int periodo5_6 = 0;
            int periodo7_8 = 0;
            var periodos = GetPeriodos(ano);
            foreach (var item in periodos)
            {
                if (item == 1 || item == 2)
                    periodo1_2 = item;
                if (item == 3 || item == 4)
                    periodo3_4 = item;
                if (item == 5 || item == 6)
                    periodo5_6 = item;
                if (item == 7 || item == 8)
                    periodo7_8 = item;
            }
            // fim do auxilio sorteador

            Individuo individuo = new Individuo();

            Random random = new Random();

            var arraySorteia = Sorteador.GetSorteia(periodo1_2, periodo3_4, periodo5_6, periodo7_8);
            arraySorteia = RemoveHorarioComReserva(arraySorteia, ano);

            while (horarios.Count > 0)
            {
                var posicaoAleatoriaArray = random.Next(0, horarios.Count); // get posicao do array horarios

                var idDisciplina = horarios.ElementAt(posicaoAleatoriaArray).IdDisciplina;
                var disciplina = getDisciplina(idDisciplina);

                var periodo = disciplina.Periodo;

                // rodar de acordo com a quantidade de horas por disciplina
                int diasDeAula = 2;
                if (disciplina.Horas == 6)
                    diasDeAula = 3;

                // laço para colocar as disciplinas em mais de 1 horario
                for (int i = 0; i < diasDeAula; i++)
                {
                    int diaSort = 0;
                    int horarioDisciplinaSort = 0;

                    if (periodo == 1 || periodo == 2)
                    {
                        (diaSort, horarioDisciplinaSort, arraySorteia) = GetSortDiaOrHorario(arraySorteia, periodo);

                        while
                            (ExisteDisciplinaNoIndividuo(individuo.Periodo_1_2, diaSort, horarioDisciplinaSort))
                        {
                            (diaSort, horarioDisciplinaSort, arraySorteia) = GetSortDiaOrHorario(arraySorteia, periodo);
                        }
                        individuo = AtribuirDisciplinaAoIndividuo(individuo, disciplina, diaSort, horarioDisciplinaSort);
                    }
                    if (periodo == 3 || periodo == 4)
                    {
                        (diaSort, horarioDisciplinaSort, arraySorteia) = GetSortDiaOrHorario(arraySorteia, periodo);
                        while
                            (ExisteDisciplinaNoIndividuo(individuo.Periodo_3_4, diaSort, horarioDisciplinaSort))
                        {
                            (diaSort, horarioDisciplinaSort, arraySorteia) = GetSortDiaOrHorario(arraySorteia, periodo);
                        }
                        individuo = AtribuirDisciplinaAoIndividuo(individuo, disciplina, diaSort, horarioDisciplinaSort);
                    }
                    if (periodo == 5 || periodo == 6)
                    {
                        (diaSort, horarioDisciplinaSort, arraySorteia) = GetSortDiaOrHorario(arraySorteia, periodo);
                        while
                            (ExisteDisciplinaNoIndividuo(individuo.Periodo_5_6, diaSort, horarioDisciplinaSort))
                        {
                            (diaSort, horarioDisciplinaSort, arraySorteia) = GetSortDiaOrHorario(arraySorteia, periodo);
                        }
                        individuo = AtribuirDisciplinaAoIndividuo(individuo, disciplina, diaSort, horarioDisciplinaSort);
                    }
                    if (periodo == 7 || periodo == 8)
                    {
                        (diaSort, horarioDisciplinaSort, arraySorteia) = GetSortDiaOrHorario(arraySorteia, periodo);
                        while
                            (ExisteDisciplinaNoIndividuo(individuo.Periodo_7_8, diaSort, horarioDisciplinaSort))
                        {
                            (diaSort, horarioDisciplinaSort, arraySorteia) = GetSortDiaOrHorario(arraySorteia, periodo);
                        }
                        individuo = AtribuirDisciplinaAoIndividuo(individuo, disciplina, diaSort, horarioDisciplinaSort);
                    }
                }

                horarios.RemoveAt(posicaoAleatoriaArray);
            }
            return individuo;
        }

        // Metodo criado para verificar a existência de uma disciplina já cadastrada no indiviuo em determinado dia e horario
        private bool ExisteDisciplinaNoIndividuo(Periodo periodo, int dia, int horarioDisciplina)
        {
            switch (dia)
            {
                case 1: // segunda
                    if (horarioDisciplina == 1) //1º horario
                        if (periodo.Segunda.Disciplina_1Horario != -1) // já existe disciplina cadastrada
                            return true;
                    if (horarioDisciplina == 2) //2º horario
                        if (periodo.Segunda.Disciplina_2Horario != -1)
                            return true;
                    if (horarioDisciplina == 3) //3º horario
                        if (periodo.Segunda.Disciplina_3Horario != -1)
                            return true;
                    break;
                case 2: // terça
                    if (horarioDisciplina == 1) //1º horario
                        if (periodo.Terca.Disciplina_1Horario != -1)
                            return true;
                    if (horarioDisciplina == 2) //2º horario
                        if (periodo.Terca.Disciplina_2Horario != -1)
                            return true;
                    if (horarioDisciplina == 3) //3º horario
                        if (periodo.Terca.Disciplina_3Horario != -1)
                            return true;
                    break;
                case 3: // quarta
                    if (horarioDisciplina == 1) //1º horario
                        if (periodo.Quarta.Disciplina_1Horario != -1)
                            return true;
                    if (horarioDisciplina == 2) //2º horario
                        if (periodo.Quarta.Disciplina_2Horario != -1)
                            return true;
                    if (horarioDisciplina == 3) //3º horario
                        if (periodo.Quarta.Disciplina_3Horario != -1)
                            return true;
                    break;
                case 4: // quinta
                    if (horarioDisciplina == 1) //1º horario
                        if (periodo.Quinta.Disciplina_1Horario != -1)
                            return true;
                    if (horarioDisciplina == 2) //2º horario
                        if (periodo.Quinta.Disciplina_2Horario != -1)
                            return true;
                    if (horarioDisciplina == 3) //3º horario
                        if (periodo.Quinta.Disciplina_3Horario != -1)
                            return true;
                    break;
                case 5: // sexta
                    if (horarioDisciplina == 1) //1º horario
                        if (periodo.Sexta.Disciplina_1Horario != -1)
                            return true;
                    if (horarioDisciplina == 2) //2º horario
                        if (periodo.Sexta.Disciplina_2Horario != -1)
                            return true;
                    if (horarioDisciplina == 3) //3º horario
                        if (periodo.Sexta.Disciplina_3Horario != -1)
                            return true;
                    break;
            }
            return false;
        }

        // retorna dia e horario respectivamente
        private (int, int, List<Sorteador>) GetSortDiaOrHorario(List<Sorteador> sorts, int periodo)
        {
            var auxSorts = sorts.Where(s => s.Periodo == periodo).ToList();
            Random random = new Random();
            var index = random.Next(0, auxSorts.Count);
            var idSorteador = auxSorts.ElementAt(index).Id;
            var sorteador = sorts.SingleOrDefault(s => s.Id == idSorteador);
            sorts.Remove(sorteador);
            return (sorteador.Dia, sorteador.Horario, sorts);
        }
        private Disciplina getDisciplina(int idDisciplina)
            => _contexto
                .Disciplinas
                .Include(d => d.Professor)
                .SingleOrDefault(d => d.Id == idDisciplina);
        private List<Horario> getHorarios(string ano)
            => _contexto
                .Horarios
                .Where(h => h.Ano.Periodo.Equals(ano))
                .ToList();
        private List<int> GetPeriodos(string ano)
            => _contexto
            .Horarios
            .Where(h => h.Ano.Periodo.Equals(ano))
            .Select(h => h.Disciplina.Periodo)
            .ToList();
        private Individuo AtribuirDisciplinaAoIndividuo(Individuo individuo, Disciplina disciplina, int dia, int horarioDisciplina)
        {
            switch (dia)
            {
                case 1: // segunda
                    if (horarioDisciplina == 1)
                    {
                        if (disciplina.Periodo == 1 || disciplina.Periodo == 2)
                            individuo.Periodo_1_2.Segunda.Disciplina_1Horario = disciplina.Id;

                        if (disciplina.Periodo == 3 || disciplina.Periodo == 4)
                            individuo.Periodo_3_4.Segunda.Disciplina_1Horario = disciplina.Id;

                        if (disciplina.Periodo == 5 || disciplina.Periodo == 6)
                            individuo.Periodo_5_6.Segunda.Disciplina_1Horario = disciplina.Id;

                        if (disciplina.Periodo == 7 || disciplina.Periodo == 8)
                            individuo.Periodo_7_8.Segunda.Disciplina_1Horario = disciplina.Id;
                    }
                    if (horarioDisciplina == 2)
                    {
                        if (disciplina.Periodo == 1 || disciplina.Periodo == 2)
                            individuo.Periodo_1_2.Segunda.Disciplina_2Horario = disciplina.Id;

                        if (disciplina.Periodo == 3 || disciplina.Periodo == 4)
                            individuo.Periodo_3_4.Segunda.Disciplina_2Horario = disciplina.Id;

                        if (disciplina.Periodo == 5 || disciplina.Periodo == 6)
                            individuo.Periodo_5_6.Segunda.Disciplina_2Horario = disciplina.Id;

                        if (disciplina.Periodo == 7 || disciplina.Periodo == 8)
                            individuo.Periodo_7_8.Segunda.Disciplina_2Horario = disciplina.Id;
                    }
                    if (horarioDisciplina == 3)
                    {
                        if (disciplina.Periodo == 1 || disciplina.Periodo == 2)
                            individuo.Periodo_1_2.Segunda.Disciplina_3Horario = disciplina.Id;

                        if (disciplina.Periodo == 3 || disciplina.Periodo == 4)
                            individuo.Periodo_3_4.Segunda.Disciplina_3Horario = disciplina.Id;

                        if (disciplina.Periodo == 5 || disciplina.Periodo == 6)
                            individuo.Periodo_5_6.Segunda.Disciplina_3Horario = disciplina.Id;

                        if (disciplina.Periodo == 7 || disciplina.Periodo == 8)
                            individuo.Periodo_7_8.Segunda.Disciplina_3Horario = disciplina.Id;
                    }
                    break;
                case 2: // terça
                    if (horarioDisciplina == 1)
                    {
                        if (disciplina.Periodo == 1 || disciplina.Periodo == 2)
                            individuo.Periodo_1_2.Terca.Disciplina_1Horario = disciplina.Id;

                        if (disciplina.Periodo == 3 || disciplina.Periodo == 4)
                            individuo.Periodo_3_4.Terca.Disciplina_1Horario = disciplina.Id;

                        if (disciplina.Periodo == 5 || disciplina.Periodo == 6)
                            individuo.Periodo_5_6.Terca.Disciplina_1Horario = disciplina.Id;

                        if (disciplina.Periodo == 7 || disciplina.Periodo == 8)
                            individuo.Periodo_7_8.Terca.Disciplina_1Horario = disciplina.Id;
                    }
                    if (horarioDisciplina == 2)
                    {
                        if (disciplina.Periodo == 1 || disciplina.Periodo == 2)
                            individuo.Periodo_1_2.Terca.Disciplina_2Horario = disciplina.Id;

                        if (disciplina.Periodo == 3 || disciplina.Periodo == 4)
                            individuo.Periodo_3_4.Terca.Disciplina_2Horario = disciplina.Id;

                        if (disciplina.Periodo == 5 || disciplina.Periodo == 6)
                            individuo.Periodo_5_6.Terca.Disciplina_2Horario = disciplina.Id;

                        if (disciplina.Periodo == 7 || disciplina.Periodo == 8)
                            individuo.Periodo_7_8.Terca.Disciplina_2Horario = disciplina.Id;
                    }
                    if (horarioDisciplina == 3)
                    {
                        if (disciplina.Periodo == 1 || disciplina.Periodo == 2)
                            individuo.Periodo_1_2.Terca.Disciplina_3Horario = disciplina.Id;

                        if (disciplina.Periodo == 3 || disciplina.Periodo == 4)
                            individuo.Periodo_3_4.Terca.Disciplina_3Horario = disciplina.Id;

                        if (disciplina.Periodo == 5 || disciplina.Periodo == 6)
                            individuo.Periodo_5_6.Terca.Disciplina_3Horario = disciplina.Id;

                        if (disciplina.Periodo == 7 || disciplina.Periodo == 8)
                            individuo.Periodo_7_8.Terca.Disciplina_3Horario = disciplina.Id;
                    }
                    break;
                case 3: // quarta
                    if (horarioDisciplina == 1)
                    {
                        if (disciplina.Periodo == 1 || disciplina.Periodo == 2)
                            individuo.Periodo_1_2.Quarta.Disciplina_1Horario = disciplina.Id;

                        if (disciplina.Periodo == 3 || disciplina.Periodo == 4)
                            individuo.Periodo_3_4.Quarta.Disciplina_1Horario = disciplina.Id;

                        if (disciplina.Periodo == 5 || disciplina.Periodo == 6)
                            individuo.Periodo_5_6.Quarta.Disciplina_1Horario = disciplina.Id;

                        if (disciplina.Periodo == 7 || disciplina.Periodo == 8)
                            individuo.Periodo_7_8.Quarta.Disciplina_1Horario = disciplina.Id;
                    }
                    if (horarioDisciplina == 2)
                    {
                        if (disciplina.Periodo == 1 || disciplina.Periodo == 2)
                            individuo.Periodo_1_2.Quarta.Disciplina_2Horario = disciplina.Id;

                        if (disciplina.Periodo == 3 || disciplina.Periodo == 4)
                            individuo.Periodo_3_4.Quarta.Disciplina_2Horario = disciplina.Id;

                        if (disciplina.Periodo == 5 || disciplina.Periodo == 6)
                            individuo.Periodo_5_6.Quarta.Disciplina_2Horario = disciplina.Id;

                        if (disciplina.Periodo == 7 || disciplina.Periodo == 8)
                            individuo.Periodo_7_8.Quarta.Disciplina_2Horario = disciplina.Id;
                    }
                    if (horarioDisciplina == 3)
                    {
                        if (disciplina.Periodo == 1 || disciplina.Periodo == 2)
                            individuo.Periodo_1_2.Quarta.Disciplina_3Horario = disciplina.Id;

                        if (disciplina.Periodo == 3 || disciplina.Periodo == 4)
                            individuo.Periodo_3_4.Quarta.Disciplina_3Horario = disciplina.Id;

                        if (disciplina.Periodo == 5 || disciplina.Periodo == 6)
                            individuo.Periodo_5_6.Quarta.Disciplina_3Horario = disciplina.Id;

                        if (disciplina.Periodo == 7 || disciplina.Periodo == 8)
                            individuo.Periodo_7_8.Quarta.Disciplina_3Horario = disciplina.Id;
                    }
                    break;
                case 4: // quinta
                    if (horarioDisciplina == 1)
                    {
                        if (disciplina.Periodo == 1 || disciplina.Periodo == 2)
                            individuo.Periodo_1_2.Quinta.Disciplina_1Horario = disciplina.Id;

                        if (disciplina.Periodo == 3 || disciplina.Periodo == 4)
                            individuo.Periodo_3_4.Quinta.Disciplina_1Horario = disciplina.Id;

                        if (disciplina.Periodo == 5 || disciplina.Periodo == 6)
                            individuo.Periodo_5_6.Quinta.Disciplina_1Horario = disciplina.Id;

                        if (disciplina.Periodo == 7 || disciplina.Periodo == 8)
                            individuo.Periodo_7_8.Quinta.Disciplina_1Horario = disciplina.Id;
                    }
                    if (horarioDisciplina == 2)
                    {
                        if (disciplina.Periodo == 1 || disciplina.Periodo == 2)
                            individuo.Periodo_1_2.Quinta.Disciplina_2Horario = disciplina.Id;

                        if (disciplina.Periodo == 3 || disciplina.Periodo == 4)
                            individuo.Periodo_3_4.Quinta.Disciplina_2Horario = disciplina.Id;

                        if (disciplina.Periodo == 5 || disciplina.Periodo == 6)
                            individuo.Periodo_5_6.Quinta.Disciplina_2Horario = disciplina.Id;

                        if (disciplina.Periodo == 7 || disciplina.Periodo == 8)
                            individuo.Periodo_7_8.Quinta.Disciplina_2Horario = disciplina.Id;
                    }
                    if (horarioDisciplina == 3)
                    {
                        if (disciplina.Periodo == 1 || disciplina.Periodo == 2)
                            individuo.Periodo_1_2.Quinta.Disciplina_3Horario = disciplina.Id;

                        if (disciplina.Periodo == 3 || disciplina.Periodo == 4)
                            individuo.Periodo_3_4.Quinta.Disciplina_3Horario = disciplina.Id;

                        if (disciplina.Periodo == 5 || disciplina.Periodo == 6)
                            individuo.Periodo_5_6.Quinta.Disciplina_3Horario = disciplina.Id;

                        if (disciplina.Periodo == 7 || disciplina.Periodo == 8)
                            individuo.Periodo_7_8.Quinta.Disciplina_3Horario = disciplina.Id;
                    }
                    break;
                case 5: // sexta
                    if (horarioDisciplina == 1)
                    {
                        if (disciplina.Periodo == 1 || disciplina.Periodo == 2)
                            individuo.Periodo_1_2.Sexta.Disciplina_1Horario = disciplina.Id;

                        if (disciplina.Periodo == 3 || disciplina.Periodo == 4)
                            individuo.Periodo_3_4.Sexta.Disciplina_1Horario = disciplina.Id;

                        if (disciplina.Periodo == 5 || disciplina.Periodo == 6)
                            individuo.Periodo_5_6.Sexta.Disciplina_1Horario = disciplina.Id;

                        if (disciplina.Periodo == 7 || disciplina.Periodo == 8)
                            individuo.Periodo_7_8.Sexta.Disciplina_1Horario = disciplina.Id;
                    }
                    if (horarioDisciplina == 2)
                    {
                        if (disciplina.Periodo == 1 || disciplina.Periodo == 2)
                            individuo.Periodo_1_2.Sexta.Disciplina_2Horario = disciplina.Id;

                        if (disciplina.Periodo == 3 || disciplina.Periodo == 4)
                            individuo.Periodo_3_4.Sexta.Disciplina_2Horario = disciplina.Id;

                        if (disciplina.Periodo == 5 || disciplina.Periodo == 6)
                            individuo.Periodo_5_6.Sexta.Disciplina_2Horario = disciplina.Id;

                        if (disciplina.Periodo == 7 || disciplina.Periodo == 8)
                            individuo.Periodo_7_8.Sexta.Disciplina_2Horario = disciplina.Id;
                    }
                    if (horarioDisciplina == 3)
                    {
                        if (disciplina.Periodo == 1 || disciplina.Periodo == 2)
                            individuo.Periodo_1_2.Sexta.Disciplina_3Horario = disciplina.Id;

                        if (disciplina.Periodo == 3 || disciplina.Periodo == 4)
                            individuo.Periodo_3_4.Sexta.Disciplina_3Horario = disciplina.Id;

                        if (disciplina.Periodo == 5 || disciplina.Periodo == 6)
                            individuo.Periodo_5_6.Sexta.Disciplina_3Horario = disciplina.Id;

                        if (disciplina.Periodo == 7 || disciplina.Periodo == 8)
                            individuo.Periodo_7_8.Sexta.Disciplina_3Horario = disciplina.Id;
                    }
                    break;
            }
            return individuo;
        }
        private List<Sorteador> RemoveHorarioComReserva(List<Sorteador> sorts, string ano)
        {
            var restricoes = GetRestricoesHorarios(ano);

            var segunda = 1;
            var terca = 2;
            var quarta = 3;
            var quinta = 4;
            var sexta = 5;

            var horario1 = 1;
            var horario2 = 2;
            var horario3 = 3;

            foreach (var item in restricoes)
            {
                if (item.SegundaHorario1)
                    sorts.Remove(sorts.SingleOrDefault(s => s.Dia == segunda && s.Horario == horario1 && s.Periodo == item.Periodo));
                if (item.SegundaHorario2)
                    sorts.Remove(sorts.SingleOrDefault(s => s.Dia == segunda && s.Horario == horario2 && s.Periodo == item.Periodo));
                if (item.SegundaHorario3)
                    sorts.Remove(sorts.SingleOrDefault(s => s.Dia == segunda && s.Horario == horario3 && s.Periodo == item.Periodo));

                if (item.TercaHorario1)
                    sorts.Remove(sorts.SingleOrDefault(s => s.Dia == terca && s.Horario == horario1 && s.Periodo == item.Periodo));
                if (item.TercaHorario2)
                    sorts.Remove(sorts.SingleOrDefault(s => s.Dia == terca && s.Horario == horario2 && s.Periodo == item.Periodo));
                if (item.TercaHorario3)
                    sorts.Remove(sorts.SingleOrDefault(s => s.Dia == terca && s.Horario == horario3 && s.Periodo == item.Periodo));

                if (item.QuartaHorario1)
                    sorts.Remove(sorts.SingleOrDefault(s => s.Dia == quarta && s.Horario == horario1 && s.Periodo == item.Periodo));
                if (item.QuartaHorario2)
                    sorts.Remove(sorts.SingleOrDefault(s => s.Dia == quarta && s.Horario == horario2 && s.Periodo == item.Periodo));
                if (item.QuartaHorario3)
                    sorts.Remove(sorts.SingleOrDefault(s => s.Dia == quarta && s.Horario == horario3 && s.Periodo == item.Periodo));

                if (item.QuintaHorario1)
                    sorts.Remove(sorts.SingleOrDefault(s => s.Dia == quinta && s.Horario == horario1 && s.Periodo == item.Periodo));
                if (item.QuintaHorario2)
                    sorts.Remove(sorts.SingleOrDefault(s => s.Dia == quinta && s.Horario == horario2 && s.Periodo == item.Periodo));
                if (item.QuintaHorario3)
                    sorts.Remove(sorts.SingleOrDefault(s => s.Dia == quinta && s.Horario == horario3 && s.Periodo == item.Periodo));

                if (item.SextaHorario1)
                    sorts.Remove(sorts.SingleOrDefault(s => s.Dia == sexta && s.Horario == horario1 && s.Periodo == item.Periodo));
                if (item.SextaHorario2)
                    sorts.Remove(sorts.SingleOrDefault(s => s.Dia == sexta && s.Horario == horario2 && s.Periodo == item.Periodo));
                if (item.SextaHorario3)
                    sorts.Remove(sorts.SingleOrDefault(s => s.Dia == sexta && s.Horario == horario3 && s.Periodo == item.Periodo));
            }
            return sorts;
        }
        private List<RestricaoHorario> GetRestricoesHorarios(string ano)
            => _contexto
            .RestricaoHorarios
            .Where(rh => rh.Ano.Periodo.Equals(ano))
            .ToList();
    }
}
