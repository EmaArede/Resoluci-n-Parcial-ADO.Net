using DronSimulator.Data.Models;
using DronSimulator.Utils;
using Npgsql;
using System;
using System.Collections.Generic;

namespace DronSimulator.Data.Repositories
{
    public class MasterRepository : IMasterRepository
    {
        private readonly string _connectionString;

        public MasterRepository()
        {
            _connectionString = ConfigurationHelper.GetPostgresConnectionString();
        }

        public int InsertMaster(MasterControl master)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = @"
                            INSERT INTO tb_master_control 
                            (fecha_ejecucion, tamanio_terreno, coordenada_x, coordenada_y)
                            VALUES (@fecha, @tamanio, @x, @y)
                            RETURNING id";

                        using (var command = new NpgsqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@fecha", master.FechaEjecucion);
                            command.Parameters.AddWithValue("@tamanio", master.TamanioTerreno);
                            command.Parameters.AddWithValue("@x", master.CoordenadaX);
                            command.Parameters.AddWithValue("@y", master.CoordenadaY);

                            int masterId = Convert.ToInt32(command.ExecuteScalar());
                            transaction.Commit();
                            return masterId;
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void InsertDetLogs(int masterId, List<DetLog> logs)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int i = 0;
                        while (i < logs.Count)
                        {
                            var log = logs[i];
                            
                            string query = @"
                                INSERT INTO tb_det_log 
                                (id_master, paso_ofuscado, coordenada_x, coordenada_y)
                                VALUES (@idMaster, @paso, @x, @y)";

                            using (var command = new NpgsqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@idMaster", masterId);
                                command.Parameters.AddWithValue("@paso", log.PasoOfuscado);
                                command.Parameters.AddWithValue("@x", log.CoordenadaX);
                                command.Parameters.AddWithValue("@y", log.CoordenadaY);

                                command.ExecuteNonQuery();
                            }
                            
                            i++;
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public List<DetLog> GetLastFiveLogs(int masterId)
        {
            var logs = new List<DetLog>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                
                string query = @"
                    SELECT id, id_master, paso_ofuscado, coordenada_x, coordenada_y
                    FROM tb_det_log
                    WHERE id_master = @masterId
                    ORDER BY id DESC
                    LIMIT 5";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@masterId", masterId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var log = new DetLog
                            {
                                Id = reader.GetInt32(0),
                                IdMaster = reader.GetInt32(1),
                                PasoOfuscado = reader.GetInt32(2),
                                CoordenadaX = reader.GetInt32(3),
                                CoordenadaY = reader.GetInt32(4)
                            };
                            logs.Add(log);
                        }
                    }
                }
            }

            return logs;
        }
    }
}