START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250514023653_remove_unused_variable_in_sm_Constructions') THEN
    ALTER TABLE "sm_Construction" DROP CONSTRAINT "FK_sm_Construction_sm_Customer_CustomerId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250514023653_remove_unused_variable_in_sm_Constructions') THEN
    DROP INDEX "IX_sm_Construction_CustomerId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250514023653_remove_unused_variable_in_sm_Constructions') THEN
    ALTER TABLE "sm_Construction" DROP COLUMN "Address";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250514023653_remove_unused_variable_in_sm_Constructions') THEN
    ALTER TABLE "sm_Construction" DROP COLUMN "ConstructionAttachments";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250514023653_remove_unused_variable_in_sm_Constructions') THEN
    ALTER TABLE "sm_Construction" DROP COLUMN "CustomerId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250514023653_remove_unused_variable_in_sm_Constructions') THEN
    ALTER TABLE "sm_Construction" DROP COLUMN "DistrictCode";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250514023653_remove_unused_variable_in_sm_Constructions') THEN
    ALTER TABLE "sm_Construction" DROP COLUMN "DistrictName";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250514023653_remove_unused_variable_in_sm_Constructions') THEN
    ALTER TABLE "sm_Construction" DROP COLUMN "EndDate";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250514023653_remove_unused_variable_in_sm_Constructions') THEN
    ALTER TABLE "sm_Construction" DROP COLUMN "ListPredicateInventory";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250514023653_remove_unused_variable_in_sm_Constructions') THEN
    ALTER TABLE "sm_Construction" DROP COLUMN "ListTeamInventory";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250514023653_remove_unused_variable_in_sm_Constructions') THEN
    ALTER TABLE "sm_Construction" DROP COLUMN "ProvinceCode";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250514023653_remove_unused_variable_in_sm_Constructions') THEN
    ALTER TABLE "sm_Construction" DROP COLUMN "ProvinceName";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250514023653_remove_unused_variable_in_sm_Constructions') THEN
    ALTER TABLE "sm_Construction" DROP COLUMN "StartDate";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250514023653_remove_unused_variable_in_sm_Constructions') THEN
    ALTER TABLE "sm_Construction" DROP COLUMN "WardCode";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250514023653_remove_unused_variable_in_sm_Constructions') THEN
    ALTER TABLE "sm_Construction" DROP COLUMN "WardName";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250514023653_remove_unused_variable_in_sm_Constructions') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250514023653_remove_unused_variable_in_sm_Constructions', '6.0.27');
    END IF;
END $EF$;
COMMIT;

