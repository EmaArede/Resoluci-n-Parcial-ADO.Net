-- Eliminar tablas si existen (para desarrollo)
DROP TABLE IF EXISTS tb_det_log;
DROP TABLE IF EXISTS tb_master_control;

-- Tabla maestra: cabecera de cada ejecución
CREATE TABLE tb_master_control (
    id SERIAL PRIMARY KEY,
    fecha_ejecucion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    tamanio_terreno INTEGER NOT NULL,
    coordenada_x INTEGER NOT NULL,
    coordenada_y INTEGER NOT NULL,
    CONSTRAINT chk_tamanio CHECK (tamanio_terreno >= 1),
    CONSTRAINT chk_coordenadas CHECK (coordenada_x >= 0 AND coordenada_y >= 0)
);

-- Tabla detalle: secuencia de movimientos
CREATE TABLE tb_det_log (
    id SERIAL PRIMARY KEY,
    id_master INTEGER NOT NULL,
    paso_ofuscado INTEGER NOT NULL,
    coordenada_x INTEGER NOT NULL,
    coordenada_y INTEGER NOT NULL,
    CONSTRAINT fk_master FOREIGN KEY (id_master) 
        REFERENCES tb_master_control(id) ON DELETE CASCADE,
    CONSTRAINT chk_paso_ofuscado CHECK (paso_ofuscado != 0)
);

-- Índice para mejorar rendimiento en consultas
CREATE INDEX idx_det_log_master ON tb_det_log(id_master);
CREATE INDEX idx_det_log_paso ON tb_det_log(paso_ofuscado);