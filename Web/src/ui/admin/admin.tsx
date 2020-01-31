import React from "react";

interface AdminState {
    
}

export class AdminPage extends React.Component<any, AdminState, any> {

    constructor(props: Readonly<any>) {
        super(props);
    }

    render() {
        return <h1>Admin</h1>;
    }
}
