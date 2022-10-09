# ArmA3SyncImporter

## Requirements
- Arma3Sync
    - Site/Wiki: http://www.sonsofexiled.fr/wiki/index.php/ArmA3Sync_Wiki_English
    - SVN: svn://www.sonsofexiled.fr/repository/ArmA3Sync/releases
- ArmA3SyncExporter
    - https://github.com/TacticalBaconDevs/ArmA3SyncExporter

## Informations
This project imports data from an Arma3Sync repo. It utilizes the [ArmA3SyncExporter](https://github.com/TacticalBaconDevs/ArmA3SyncExporter) project to get the right output.

## Examples
```
List<Event?> events = ImporterUtils.GetEvent(http://repo.tacticalbacon.de/.a3s/events); // or http://repo.tacticalbacon.de/.a3s/autoconfig
List<Addon?> remoteAddons = ImporterUtils.GetRemoteSync(http://repo.tacticalbacon.de/.a3s/sync); // or http://repo.tacticalbacon.de/.a3s/autoconfig
List<Addon?> localAddons = ImporterUtils.GetLokalSync(@"C:\Arma3\Mods");
```
