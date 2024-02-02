import React from 'react';
import { AppearanceProvider } from 'react-native-appearance';
import { SafeAreaProvider } from 'react-native-safe-area-context';
import { ApplicationProvider, IconRegistry } from '@ui-kitten/components';
import { EvaIconsPack } from '@ui-kitten/eva-icons';
import { LoadFontsTask, Task } from './src/app/app-loading.component';
import { StatusBar, Text } from 'react-native';
import { AppIconsPack } from './src/app/app-icons-pack';
import { AppLoading } from './src/app/app-loading.component.expo';
import { appMappings, appThemes } from './src/app/app-theming';
import { AppStorage } from './src/services/app-storage.service';
import { Mapping, Theme, Theming } from './src/services/theme.service';
import { SplashImage } from './src/components/splash-image.component';

// import { SplashImage } from '../components/splash-image.component';


const loadingTasks: Task[] = [
  () => LoadFontsTask({
    'opensans-regular': require('./src/assets/fonts/opensans-regular.ttf'),
    'roboto-regular': require('./src/assets/fonts/roboto-regular.ttf'),
  }),
  () => AppStorage.getMapping(defaultConfig.mapping).then(result => ['mapping', result]),
  () => AppStorage.getTheme(defaultConfig.theme).then(result => ['theme', result]),
];

const defaultConfig: { mapping: Mapping, theme: Theme } = {
  mapping: 'eva',
  theme: 'light',
};


const App: React.FC<{ mapping: Mapping, theme: Theme }> = ({ mapping, theme }) => {

  const [mappingContext, currentMapping] = Theming.useMapping(appMappings, mapping);
  const [themeContext, currentTheme] = Theming.useTheming(appThemes, mapping, theme);

  return (
    <React.Fragment>
      <IconRegistry icons={[EvaIconsPack, AppIconsPack]} />
      <AppearanceProvider>
        <ApplicationProvider {...currentMapping} theme={currentTheme}>
          <Theming.MappingContext.Provider value={mappingContext}>
            <Theming.ThemeContext.Provider value={themeContext}>
              <SafeAreaProvider>
                <StatusBar />
                <Text>HELLO WORLD</Text>
                {/* <AppNavigator /> */}
              </SafeAreaProvider>
            </Theming.ThemeContext.Provider>
          </Theming.MappingContext.Provider>
        </ApplicationProvider>
      </AppearanceProvider>
    </React.Fragment>
  );
};

const Splash = ({ loading }: {loading: any}): React.ReactElement => (
  <SplashImage
    loading={loading}
    source={require('./src/assets/images/image-splash.png')}
  />
);

export default (): React.ReactElement => (
  <AppLoading
    tasks={loadingTasks}
    initialConfig={defaultConfig}
    placeholder={Splash}>
    {props => <App {...props} />}
  </AppLoading>
);
