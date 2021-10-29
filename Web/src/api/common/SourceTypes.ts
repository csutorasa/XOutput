export enum SourceTypes {
    None = 0,
    Button = 1,
    Slider = 2,
    Dpad = 4,
    AxisX = 8,
    AxisY = 16,
    AxisZ = 32,
    Axis = AxisX | AxisY | AxisZ,
};
