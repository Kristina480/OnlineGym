TRUNCATE TABLE
    messages,
    notifications,
    reports,
    memberships,
    ratings,
    workout_items,
    workouts,
    exercises,
    machines,
    equipment,
    collaborations,
    collaboration_requests,
    pricing_packages,
    licenses,
    registration_requests,
    administrators,
    clients,
    trainers,
    user_accounts
RESTART IDENTITY CASCADE;

INSERT INTO user_accounts
(username, password, user_type)
VALUES
    ('admin', 'admin123', 'ADMIN'),
    ('marko', '123', 'TRAINER'),
    ('ana', '123', 'TRAINER'),
    ('milan', '123', 'CLIENT'),
    ('jelena', '123', 'CLIENT'),
    ('nikola', '123', 'CLIENT');

INSERT INTO administrators
(account_id)
VALUES
    (1);

INSERT INTO trainers
(account_id, first_name, last_name, specialization, average_rating, education, recommendations)
VALUES
    (2, 'Marko', 'Marković', 'Bodybuilding', 4.80, 'FTN', 'Odličan trener'),
    (3, 'Ana', 'Jovanović', 'Fitness', 4.95, 'DIF', 'Veliko iskustvo');

INSERT INTO clients
(account_id, first_name, last_name, height, weight, goal, health_issues, workouts_per_week)
VALUES
    (4, 'Milan', 'Petrović', 185, 90, 'Mršavljenje', NULL, 3),
    (5, 'Jelena', 'Nikolić', 170, 60, 'Kondicija', NULL, 4),
    (6, 'Nikola', 'Ilić', 178, 82, 'Mišićna masa', 'Povreda kolena', 5);

INSERT INTO messages
(sender_account_id, recipient_account_id, content, is_read, sent_at)
VALUES
    (4, 2, 'Zdravo, želeo bih da zakažem prvi trening.', TRUE, '2026-07-02 10:00:00'),
    (2, 4, 'Naravno. Možemo da počnemo u petak u 18 časova.', TRUE, '2026-07-02 10:15:00'),
    (5, 3, 'Da li možemo da pomerimo sledeći trening na 19 časova?', FALSE, '2026-07-09 12:30:00'),
    (3, 5, 'Može, termin u 19 časova mi odgovara.', FALSE, '2026-07-09 12:45:00');

INSERT INTO registration_requests
(trainer_id, administrator_id, request_date, status)
VALUES
    (1, 1, '2026-06-20', 'APPROVED'),
    (2, 1, '2026-06-21', 'APPROVED');

INSERT INTO licenses
(trainer_id, name, document_type, issue_date)
VALUES
    (1, 'Personalni trener', 'PDF', '2024-01-15'),
    (2, 'Fitnes instruktor', 'PDF', '2023-10-01');

INSERT INTO pricing_packages
(trainer_id, workouts_per_week, monthly_price)
VALUES
    (1, 3, 12000),
    (1, 5, 18000),
    (2, 3, 11000),
    (2, 5, 17000);

INSERT INTO collaboration_requests
(client_id, trainer_id, request_date, status)
VALUES
    (1, 1, '2026-06-25', 'APPROVED'),
    (2, 2, '2026-06-26', 'APPROVED'),
    (3, 1, '2026-07-05', 'PENDING');

INSERT INTO collaborations
(trainer_id, client_id, request_id, pricing_package_id, start_date, end_date, status, workouts_per_week, monthly_price)
VALUES
    (1, 1, 1, 1, '2026-07-01', NULL, 'ACTIVE', 3, 12000),
    (2, 2, 2, 4, '2026-07-01', NULL, 'ACTIVE', 5, 17000);

INSERT INTO equipment
(name)
VALUES
    ('Tegovi'),
    ('Šipka'),
    ('Prostirka');

INSERT INTO machines
(name)
VALUES
    ('Bench klupa'),
    ('Smit mašina'),
    ('Leg press');

INSERT INTO exercises
(trainer_id, equipment_id, machine_id, name, video_url)
VALUES
    (1, 1, 1, 'Bench press', NULL),
    (1, 2, 2, 'Čučanj', NULL),
    (1, 2, NULL, 'Mrtvo dizanje', NULL),
    (2, NULL, NULL, 'Iskorak', NULL),
    (2, 3, NULL, 'Sklekovi', NULL);

INSERT INTO workouts
(collaboration_id, scheduled_at, status, is_completed, comment, workout_rating)
VALUES
    (1, '2026-07-10 18:00:00', 'COMPLETED', TRUE, 'Odličan trening', 5),
    (1, '2026-07-12 18:00:00', 'SCHEDULED', FALSE, NULL, NULL),
    (2, '2026-07-11 19:00:00', 'COMPLETED', TRUE, 'Vidljiv napredak', 4);

INSERT INTO workout_items
(workout_id, exercise_id, repetition_count, is_completed, item_rating, comment)
VALUES
    (1, 1, 10, TRUE, 5, 'Odlično izvedeno'),
    (1, 2, 12, TRUE, 4, 'Potrebno malo dublje spuštanje'),
    (2, 3, 8, FALSE, NULL, NULL),
    (3, 4, 15, TRUE, 4, 'Stabilno izvođenje'),
    (3, 5, 20, TRUE, 5, 'Bez problema završeno');

INSERT INTO ratings
(client_id, trainer_id, rating, comment, rating_date)
VALUES
    (1, 1, 5, 'Odličan trener', '2026-07-10'),
    (2, 2, 5, 'Sve preporuke', '2026-07-11'),
    (3, 1, 4, 'Zadovoljan saradnjom', '2026-07-08');

INSERT INTO memberships
(collaboration_id, billing_month, payment_date, amount, is_paid)
VALUES
    (1, '2026-07-01', '2026-07-02', 12000, TRUE),
    (2, '2026-07-01', '2026-07-03', 17000, TRUE);

INSERT INTO reports
(client_id, trainer_id, administrator_id, description, report_date, status, resolution)
VALUES
    (1, 2, 1, 'Trener je kasnio na trening.', '2026-07-08', 'PENDING', NULL),
    (2, 1, 1, 'Trener nije odgovarao na poruke.', '2026-07-07', 'APPROVED',
     'Administrator je upozorio trenera.');

INSERT INTO notifications
(trainer_id, content, notification_date, type)
VALUES
    (1, 'Imate novu prijavu.', '2026-07-08', 'PRIJAVA'),
    (2, 'Nova saradnja je odobrena.', '2026-06-26', 'SARADNJA');